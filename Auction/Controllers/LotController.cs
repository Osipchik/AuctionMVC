using System;
using System.Threading.Tasks;
using Auction.DTO;
using Auction.DTO.Pagination;
using Auction.Models;
using Auction.Services;
using Auction.Services.CloudStorage;
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
                var lot = new Lot {AppUserId = HttpContext.UserId(), CreatedAt = DateTime.Now};
            
                SetLotModel(model, lot);
                if (model.Image != null)
                {
                    await AddImage(lot, model.Image);
                }
            
                await _repository.Add(lot);

                return RedirectToAction("Get", lot.Id);
            }
            
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Update(int lotId)
        {
            var model = await GetModel(lotId);
            if (model != null)
            {
                ViewData["Id"] = lotId;
                return View(model);
            }
            
            return RedirectToAction("index", "Home");
        }
        
        private async Task<CreateLotModel> GetModel(int id)
        {
            var lot = await _repository.Find(id);
            if (lot != null)
            {
                var model = new CreateLotModel
                {
                    Title = lot.Title,
                    Description = lot.Description,
                    ImageUrl = lot.ImageUrl,
                    LunchAt = lot.LunchAt,
                    EndAt = lot.EndAt,
                    Goal = lot.Goal,
                    Story = lot.Story
                };
                
                return model;
            }

            return null;
        }
        
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int lotId, CreateLotModel model)
        {
            if (ModelState.IsValid)
            {
                var lot = User.IsInRole(Constants.AdminRole)
                    ? await _repository.Find(lotId)
                    : await _repository.FindUserLot(lotId, HttpContext.UserId());

                if (lot != null)
                {
                    SetLotModel(model, lot);
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
            var lot = User.IsInRole(Constants.AdminRole)
                ? await _repository.Find(lotId)
                : await _repository.FindUserLot(lotId, HttpContext.UserId());

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
            var isValid = lot != null; //&& lot.AppUserId == HttpContext.UserId() || User.IsInRole(Constants.AdminRole);
            if (isValid)
            {
                ViewData["UserId"] = HttpContext.UserId();
                return View(lot);
            }
            
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RenderMarkdown([FromBody] string markdown)
        {
            return PartialView("_Markdown", markdown);
        }
    }
}