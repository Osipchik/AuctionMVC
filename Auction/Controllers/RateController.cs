using System;
using System.Linq;
using System.Threading.Tasks;
using Auction.Data;
using Auction.Models;
using Auction.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auction.Controllers
{
    public class RateController: ControllerBase
    {
        private readonly ILotRepository _lotRepository;
        private readonly IRepository<Rate> _repository;

        public RateController(ILotRepository lotRepository, IRepository<Rate> repository)
        {
            _lotRepository = lotRepository;
            _repository = repository;
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetRate(int lotId, decimal rate)
        {
            var lot = await _lotRepository.Find(lotId);
            
            if (lot != null)
            {
                await _lotRepository.Context.Entry(lot).Collection(i => i.Rates).LoadAsync();

                if (rate > lot.Rates.Max(i => i.Amount))
                {
                    var newRate = new Rate
                    {
                        Amount = rate,
                        CreatedAt = DateTime.UtcNow,
                        LotId = lotId,
                        AppUserId = HttpContext.UserId()
                    };

                    newRate = await _repository.Add(newRate);
                    return Ok(newRate);
                }
            }

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> GetLotFunding(int lotId)
        {
            var lot = await _lotRepository.Find(lotId);
            await _lotRepository.Context.Entry(lot).Collection(i => i.Rates).LoadAsync();
            
            return Ok(new
            {
                ratesCount = lot.Rates.Count,
                currentPrice = lot.Rates.OrderByDescending(i => i.CreatedAt).FirstOrDefault()?.Amount ?? 0m
            });
        }
    }
}