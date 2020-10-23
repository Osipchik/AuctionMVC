using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository;
using Repository.Interfaces;
using Service;
using Service.Interfaces;
using Web.DTO;
using Web.DTO.Lot;

namespace Web.Controllers
{
    public class LotController: Controller
    {
        private readonly ILotRepository _repository;
        private readonly ICloudStorage _cloudStorage;
        
        public LotController(ILotRepository repository, ICloudStorage cloudStorage)
        {
            _repository = repository;
            _cloudStorage = cloudStorage;
            
            
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
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
            
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Update(int lotId)
        {
            var lot = await _repository.Find(lotId, HttpContext);
            if (lot != null)
            {
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
                var lot = await _repository.Find(lotId, HttpContext);
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
            var lot = await _repository.Find(lotId, HttpContext);

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
                    return View(lot);
                }
            }
            
            return RedirectToAction("Error", "Home");
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
            var lot = await _repository.Find(lotId, HttpContext);
            if (lot != null && IsLotReady(lot))
            {
                lot.IsAvailable = true;
                await _repository.Update(lot);
      
                // BackgroundJob.Schedule(
                //     () => BackgroundTask(lotId),
                //     lot.EndAt - DateTime.UtcNow
                // );
                
                return Ok(new {lotId});
            }
            
            return Accepted();
        }

        private async Task BackgroundTask(int id)
        {
            var lot = await _repository.Find(id);
            await _repository.Delete(lot);
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