using System;
using System.Linq;
using System.Threading.Tasks;
using Auction.DTO;
using Auction.Models;
using Auction.Services;
using Auction.Services.CloudStorage;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auction.Controllers
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
                    CreatedAt = DateTime.Now
                };
            
                SetLotModel(model, lot);
                await AddImage(lot, model.Image);
                await _repository.Add(lot);

                return RedirectToAction("Get", lot.Id);
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
            }
            
            return RedirectToAction("Get", new {lotId});
        }
        
        private void SetLotModel(CreateLotModel model, Lot lot)
        {
            lot.Title = model.Title;
            lot.Description = model.Description;
            lot.LunchAt = model.LunchAt;
            lot.EndAt = model.EndAt;
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
                    await _repository.Context.Entry(lot).Collection(i => i.Rates).LoadAsync();
                    var amount = lot.Rates.OrderByDescending(c => c.CreatedAt).FirstOrDefault()?.Amount;
                    lot.Funded = amount ?? 0m;
                
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
            if (lot != null)
            {
                lot.IsAvailable = true;
                await _repository.Update(lot);

                BackgroundJob.Schedule(
                    () => BackgroundTask(lotId),
                    lot.EndAt - DateTime.UtcNow
                );
            }
            
            return Ok(new {lotId});
        }

        private async Task BackgroundTask(int id)
        {
            var lot = await _repository.Find(id);
            await _repository.Delete(lot);
        }
    }
}