using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repository;
using Repository.Interfaces;
using Repository.SortOptions;
using Web.DTO;
using Web.DTO.Pagination;


namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILotRepository _repository;
        private readonly int _pageSize;
        
        public HomeController(ILotRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _pageSize  = Convert.ToInt32(configuration.GetSection("PageSize").Value);
        }
        
        [HttpGet]
        public async Task<IActionResult> Index(string search, SortBy sortBy, ShowOptions show, int page = 1)
        {
            var (lots, totalItems) = await GetLots(sortBy, show, search, page);
            var viewModel = BuildIndexViewModel(lots, page, sortBy, show, search, totalItems);
            
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetPage(string search, SortBy sortBy, ShowOptions show, int page = 1)
        {
            var (lots, totalItems) = await GetLots(sortBy, show, search, page);
            var viewModel = BuildIndexViewModel(lots, page, sortBy, show, search, totalItems);
            
            return PartialView("_LotView", viewModel);
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




        private async Task<(IEnumerable<LotView> lots, int)> GetLots(SortBy sortBy,
            ShowOptions show, string search, int page)
        {
            var query = _repository.FilterLots(sortBy, show);

            var asd = query.ToList();
            
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

            var lots = await _repository.FindRange(query, _pageSize, (page - 1) * _pageSize);

            return (lots, query.Count());
        }

        private IndexViewModel BuildIndexViewModel(IEnumerable<LotView> lots, int page, SortBy sortBy,
            ShowOptions show, string search, int totalItems)
        {
            var viewModel = new IndexViewModel
            {
                Lots = lots,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = _pageSize,
                    TotalItems = totalItems
                },
                SortBy = sortBy,
                ShowOptions = show,
                Search = search
            };

            return viewModel;
        }
    }
}