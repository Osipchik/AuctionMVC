﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repository.Interfaces;
using Repository.SortOptions;
using Web.DTO;
using Web.DTO.Pagination;


namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILotRepository _repository;
        private readonly ICategoryRepository _categoryRepository;
        
        public HomeController(ILotRepository repository, ICategoryRepository categoryRepository)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index(string search, SortBy sortBy, ShowOptions show, int categoryId)
        {
            var categories = await _categoryRepository.GetAll();
            var viewModel = BuildIndexViewModel(categoryId, sortBy, show, search, categories);
            
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> LoadLots(string search, int categoryId, SortBy sortBy, ShowOptions show, int take, int skip)
        {
            var query = _repository.FilterLots(sortBy, show, categoryId);
            
            if (show == ShowOptions.MyLots)
            {
                query = HttpContext.User.Identity.IsAuthenticated
                    ? query.Where(i => i.AppUserId == HttpContext.UserId())
                    : query.Where(i => i.IsAvailable);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = search[0] == '@'
                    ? query.Where(i => i.AppUser.UserName.Contains(search.Substring(1)))
                    : query.Where(i => i.Title.Contains(search));
            }

            var lots = await _repository.FindRange(query, take, skip);
            
            return PartialView("_LotList", lots);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        public IActionResult NotFoundError(NotfoundErrorViewModel model)
        {
            return View("404", model);
        }
        
        private IndexViewModel BuildIndexViewModel(int categoryId, SortBy sortBy,
            ShowOptions show, string search, IEnumerable<Category> categories)
        {
            var viewModel = new IndexViewModel
            {
                SortBy = sortBy,
                ShowOptions = show,
                Search = search,
                CategoryId = categoryId,
                Categories = new SelectList(categories, "Id", "Name")
            };

            return viewModel;
        }
    }
}