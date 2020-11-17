using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Repository.Interfaces;

namespace Web.Hubs
{
    public class BetHub : Hub
    {
        private readonly IRepository<Rate> _repository;
        private readonly ILotRepository _lotRepository;

        public BetHub(IRepository<Rate> repository, ILotRepository lotRepository)
        {
            _repository = repository;
            _lotRepository = lotRepository;
        }

        [Authorize]
        public async Task AddBet(string lotId, string bet)
        {
            var betM = decimal.Parse(bet, CultureInfo.InvariantCulture);
            var id = int.Parse(lotId);

            var newBet = await SaveBet(id, betM);
            var task = newBet != null
                ? Clients.Group(lotId).SendAsync("UpdateBet", newBet)
                : Clients.Caller.SendAsync("Exception", "The object has not preserved");
        }

        public Task JoinRoom(string lotId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, lotId);
        }

        public Task LeaveRoom(string lotId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, lotId);
        }

        private async Task<Rate> SaveBet(int lotId, decimal bet)
        {
            var lot = await _lotRepository.Find(lotId);
            if (lot != null)
            {
                await _lotRepository.LoadRates(lot);
                if (lot.Rates.Count == 0 || bet > lot.Rates.Max(i => i.Amount))
                {
                    var rate = new Rate
                    {
                        Amount = bet,
                        CreatedAt = DateTime.UtcNow,
                        LotId = lotId,
                        AppUserId = Context.UserIdentifier
                    };

                    return await _repository.Add(rate);
                }
            }

            return null;
        }
    }
}