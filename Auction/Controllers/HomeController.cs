using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Auction.Data;
using Auction.DTO.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Auction.Models;
using Auction.Services;
using Microsoft.EntityFrameworkCore;

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
        
        public async Task<IActionResult> Index(int page = 1)
        {
            var viewModel = new IndexViewModel
            {
                Lots = await _repository.FindRange(pageSize, (page - 1) * pageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = await _repository.Count()
                }
            };
            return View(viewModel);
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