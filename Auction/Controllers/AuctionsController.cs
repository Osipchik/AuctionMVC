using System.Threading.Tasks;
using Auction.Models;
using Auction.Services;
using Microsoft.AspNetCore.Mvc;

namespace Auction.Controllers
{
    public class AuctionsController: Controller
    {
        private readonly IRepository<Lot> _repository;

        public AuctionsController(IRepository<Lot> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int lotId)
        {
            var lot = await _repository.Find(lotId);
            if (lot != null)
            {
                return View(lot);   
            }

            return RedirectToAction("Index", "Home");
        }
    }
}