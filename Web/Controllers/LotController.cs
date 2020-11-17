using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repository;
using Repository.Interfaces;
using Service;
using Service.Interfaces;
using Web.DTO;
using Web.DTO.Lot;
using Web.EmailSender;
using MailMessage = Web.EmailSender.MailMessage;

namespace Web.Controllers
{
    public class LotController: Controller
    {
        private readonly ILotRepository _repository;
        private readonly ICloudStorage _cloudStorage;
        private readonly IEmailService _emailService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICategoryRepository _categoryRepository;
        
        public LotController(ILotRepository repository,
            ICloudStorage cloudStorage, 
            IEmailService emailService, 
            UserManager<AppUser> userManager, 
            ICategoryRepository categoryRepository)
        {
            _repository = repository;
            _cloudStorage = cloudStorage;
            _emailService = emailService;
            _userManager = userManager;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAll();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");;
            
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateLotModel model)
        {
            if (ModelState.IsValid)
            {
                var lot = new Lot
                {
                    AppUserId = HttpContext.UserId(),
                    CreatedAt = DateTime.UtcNow
                };
            
                SetLotModel(model, lot);
                await AddImage(lot, model.Image);
                await _repository.Add(lot);

                return RedirectToAction("Index", "Home");
            }
            
            ModelState.AddModelError(string.Empty, "Fill the blanks");
            
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Update(int lotId)
        {
            var lot = await _repository.Find(lotId, HttpContext.UserId(), HttpContext.User.IsInRole(Constants.AdminRole));
            if (lot != null)
            {
                var categories = await _categoryRepository.GetAll();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
                ViewData["Id"] = lotId;
                return View( new CreateLotModel(lot));
            }
            
            return RedirectToAction("index", "Home");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int lotId, CreateLotModel model)
        {
            if (ModelState.IsValid)
            {
                var lot = await _repository.Find(lotId, HttpContext.UserId(), HttpContext.User.IsInRole(Constants.AdminRole));
                if (lot != null)
                {
                    SetLotModel(model, lot);
                    await AddImage(lot, model.Image);
                    await _repository.Update(lot);   
                }
                
                return RedirectToAction("Get", new {lotId});
            }

            return View(model);
        }
        
        private void SetLotModel(CreateLotModel model, Lot lot)
        {
            lot.Title = model.Title;
            lot.Description = model.Description;
            lot.LunchAt = model.LunchAt.ToUniversalTime();
            lot.EndAt = model.EndAt.ToUniversalTime();
            lot.CategoryId = model.CategoryId;
            lot.Goal = model.Goal;
            lot.Story = model.Story;
        }

        private async Task AddImage(Lot lot, IFormFile image)
        {
            if (image == null)
            {
                return;
            }
            
            if (lot.ImageUrl != null)
            {
                await _cloudStorage.DeleteFileAsync(lot.ImageUrl);
            }
            
            var filename = _cloudStorage.CreateFileName(image, HttpContext.UserId());
            lot.ImageUrl = await _cloudStorage.UploadFileAsync(image, filename);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int lotId)
        {
            var lot = await _repository.Find(lotId, HttpContext.UserId(), HttpContext.User.IsInRole(Constants.AdminRole));

            if (lot != null)
            {
                await _repository.Delete(lot);
            }
            
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Get(int lotId)
        {
            var lot = await _repository.Find(lotId);
            
            if (lot != null)
            {
                var isOwner = HttpContext.User.Identity.IsAuthenticated &&
                              (lot.AppUserId == HttpContext.UserId() || User.IsInRole(Constants.AdminRole));
                
                if (lot.IsAvailable || isOwner)
                {
                    await _repository.LoadRates(lot);
                    ViewData["UserId"] = HttpContext.UserId();
                    ViewData["Funded"] = lot.Rates?.OrderByDescending(c => c.CreatedAt).FirstOrDefault()?.Amount ?? 0m;
                    return View(lot);
                }
            }
            
            
            // var errorViewModel = new NotfoundErrorViewModel
            // {
            //     Message = $"Lot with Id {lotId} not found"
            // };

            ViewBag.Message = $"Lot with Id {lotId} not found";

            return View("ErrorPage");
            // return RedirectToAction("NotFoundError", "Home", errorViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RenderMarkdown([FromBody] string markdown)
        {
            return PartialView("_Markdown", markdown);
        }

        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LaunchProject(int lotId)
        {
            var lot = await _repository.Find(lotId, HttpContext.UserId(), HttpContext.User.IsInRole(Constants.AdminRole));
            if (lot != null && IsLotReady(lot))
            {
                lot.IsAvailable = true;
                await _repository.Update(lot);

                BackgroundJob.Schedule(
                    () => SendFinishedNotification(lotId),
                    lot.EndAt - DateTime.UtcNow
                );
                
                return Ok();
            }
            
            return BadRequest();
        }

        private async Task SendLaunchNotification(int id)
        {
            var lot = await _repository.Find(id);
            if (lot != null)
            {
                var user = await _userManager.FindByIdAsync(lot.AppUserId);
                await _emailService.Send(new MailMessage(user.Email, user.UserName, $"{lot.Title} is successfully started",
                    EmailTypes.LaunchNotification));
            }
        }

        private async Task SendFinishedNotification(int id)
        {
            var lot = await _repository.Find(id);
            if (lot != null)
            {
                lot.IsAvailable = false;
                await _repository.Update(lot);
                await _repository.LoadRates(lot);
                var maxRate = lot.Rates[0];

                var betOwner = await _userManager.FindByIdAsync(maxRate?.AppUserId);
                var user = await _userManager.FindByIdAsync(lot.AppUserId);

                var message = $"{lot.Title} is finished!\n";
                
                message += betOwner == null 
                    ? "Unfortunately, no one users made a bets"
                    : $"{betOwner.UserName.Split('@')[0]} made the max bet: " +
                      $"{lot.Rates.OrderByDescending(c => c.CreatedAt).First().Amount}";
                
                
                await _emailService.Send(new MailMessage(user.Email, user.UserName, message, EmailTypes.FinishNotification));
            }
        }

        private bool IsLotReady(Lot lot)
        {
            var model = new LotModel(lot);

            var isAnyEmpty = model.GetType().GetProperties()
                .Where(i => i.PropertyType == typeof(string))
                .Select(i => (string) i.GetValue(model))
                .Any(string.IsNullOrEmpty);

            var isDateValid = model.LunchAt > DateTime.UtcNow || model.EndAt <= model.LunchAt.AddHours(4);

            return !isAnyEmpty && isDateValid;
        }
    }
}