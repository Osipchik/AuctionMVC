﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain.Core;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Domain.Interfaces;
using Infrastructure.Data.SortOptions;
using Web.DTO.Lot;
using Web.EmailSender;
using MailMessage = Web.EmailSender.MailMessage;

namespace Web.Controllers
{
    public class LotController: Controller
    {
        private readonly ILotRepository<SortBy, ShowOptions> _repository;
        private readonly ICloudStorage _cloudStorage;
        private readonly IEmailSender<MailMessage> _emailSender;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICategoryRepository _categoryRepository;
        
        public LotController(ILotRepository<SortBy, ShowOptions> repository,
            ICloudStorage cloudStorage, 
            IEmailSender<MailMessage> emailSender, 
            UserManager<AppUser> userManager, 
            ICategoryRepository categoryRepository)
        {
            _repository = repository;
            _cloudStorage = cloudStorage;
            _emailSender = emailSender;
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
            var categories = await _categoryRepository.GetAll();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            
            return View(model);
        }
        
        private void SetLotModel(CreateLotModel model, Lot lot)
        {
            lot.Title = model.Title;
            lot.Description = model.Description;
            lot.EndAt = model.EndAt.ToUniversalTime();
            lot.CategoryId = model.CategoryId;
            lot.MinPrice = model.MinPrice;
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

            await using var memoryStream = new MemoryStream();
            await image.CopyToAsync(memoryStream);
            
            var filename = _cloudStorage.CreateFileName(image.FileName, HttpContext.UserId());
            lot.ImageUrl = await _cloudStorage.UploadFileAsync(memoryStream, filename, image.ContentType);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int lotId)
        {
            var lot = await _repository.Find(lotId, HttpContext.UserId(), HttpContext.User.IsInRole(Constants.AdminRole));

            if (lot != null)
            {
                await _cloudStorage.DeleteFileAsync(lot.ImageUrl);
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
            
            ViewBag.Message = $"Lot with Id {lotId} not found";

            return View("ErrorPage");
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
                lot.LunchAt = DateTime.UtcNow;
                await _repository.Update(lot);
            
                BackgroundJob.Schedule(
                    () => SendFinishedNotification(lotId),
                    lot.EndAt - DateTime.UtcNow
                );
                
                return Ok();
            }
            
            return Accepted();
        }

        public async Task SendLaunchNotification(int id)
        {
            var lot = await _repository.Find(id);
            if (lot != null)
            {
                var user = await _userManager.FindByIdAsync(lot.AppUserId);
                await _emailSender.Send(new MailMessage(user.Email, user.UserName, $"{lot.Title} is successfully started",
                    EmailTypes.LaunchNotification));
            }
        }

        public async Task SendFinishedNotification(int id)
        {
            var lot = await _repository.Find(id);
            if (lot != null)
            {
                lot.IsAvailable = false;
                await _repository.Update(lot);
                await _repository.LoadRates(lot);

                var user = await _userManager.FindByIdAsync(lot.AppUserId);
                var message = $"{lot.Title} is finished!\n";;
                
                if (lot.Rates.Count > 0)
                {
                    var maxRate = lot.Rates[0];

                    var betOwner = await _userManager.FindByIdAsync(maxRate.AppUserId);
                    message += betOwner == null 
                        ? "Unfortunately, bet owner delete account"
                        : $"{betOwner.UserName.Split('@')[0]} made the max bet: " +
                          $"{lot.Rates.OrderByDescending(c => c.CreatedAt).First().Amount}";
                }
                else
                {
                    message += "Unfortunately, no one users made a bets";
                }

                await _emailSender.Send(new MailMessage(user.Email, user.UserName, message, EmailTypes.FinishNotification));
            }
        }

        private bool IsLotReady(Lot lot)
        {
            var model = new LotModel(lot);

            var isAnyEmpty = model.GetType().GetProperties()
                .Where(i => i.PropertyType == typeof(string))
                .Select(i => (string) i.GetValue(model))
                .Any(string.IsNullOrEmpty);

            var isDateValid = model.EndAt >= DateTime.UtcNow.AddHours(1);

            return !isAnyEmpty && isDateValid;
        }
    }
}