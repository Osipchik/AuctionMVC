using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Auction.Data;
using Auction.DTO.Pagination;
using Auction.DTO.SortOptions;
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
        
        [HttpGet]
        public async Task<IActionResult> Index(IndexViewModel model, int page = 1)
        {
            var sortModel = model.SortViewModel ?? new SortViewModel(); 
            
            var viewModel = new IndexViewModel
            {
                Lots = await _repository.Order(pageSize, (page - 1) * pageSize, sortModel.SortBy, i => true),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = await _repository.Count()
                },
                SortViewModel = sortModel
            };
            return View(viewModel);
        }

        private void Sort(SortBy sort)
        {
            
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