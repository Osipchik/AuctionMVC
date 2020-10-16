using System.Diagnostics;
using System.Threading.Tasks;
using Auction.DTO.Pagination;
using Auction.DTO.SortOptions;
using Microsoft.AspNetCore.Mvc;
using Auction.Models;
using Auction.Services;

namespace Auction.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILotRepository _repository;
        public int pageSize = 1;
        
        public HomeController(ILotRepository repository)
        {
            _repository = repository;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index(SortBy sortBy, ShowOptions show, int page = 1)
        {
            return View(await CreateViewModel(sortBy, show, page));
        }

        [HttpGet]
        public async Task<IActionResult> GetPage(SortBy sortBy, ShowOptions show, int page = 1)
        {
            return PartialView("_LotView", await CreateViewModel(sortBy, show, page));
        }

        private async Task<IndexViewModel> CreateViewModel(SortBy sortBy, ShowOptions show, int page = 1)
        {
            var query = _repository.FilterLots(sortBy, show, HttpContext);
            
            var viewModel = new IndexViewModel
            {
                Lots = await _repository.FindRange(query, pageSize, (page - 1) * pageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = await _repository.Count()
                },
                SortBy = sortBy,
                ShowOptions = show
            };

            return viewModel;
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
    }
}