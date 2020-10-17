using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.SortOptions;
using Web.DTO;
using Web.DTO.Pagination;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILotRepository _repository;
        private readonly IMapper _mapper;
        public int pageSize = 1;
        
        public HomeController(ILotRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index(string search, SortBy sortBy, ShowOptions show, int page = 1)
        {
            return View(await CreateViewModel(search, sortBy, show, page));
        }

        [HttpGet]
        public async Task<IActionResult> GetPage(string search, SortBy sortBy, ShowOptions show, int page = 1)
        {
            return PartialView("_LotView", await CreateViewModel(search, sortBy, show, page));
        }

        private async Task<IndexViewModel> CreateViewModel(string search, SortBy sortBy, ShowOptions show, int page = 1)
        {
            var query = _repository.FilterLots(sortBy, show, HttpContext);

            if (!string.IsNullOrEmpty(search))
            {
                query = search[0] == '@'
                    ? query.Include(i => i.AppUser).Where(i => i.AppUser.UserName.Contains(search.Substring(1)))
                    : query.Where(i => i.Title.Contains(search));
            }

            var lots = await _repository.FindRange(query, pageSize, (page - 1) * pageSize);
            
            var viewModel = new IndexViewModel
            {
                Lots = lots.Select(_mapper.Map<Lot, LotPreview>).ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = await query.CountAsync()
                },
                SortBy = sortBy,
                ShowOptions = show,
                Search = search
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