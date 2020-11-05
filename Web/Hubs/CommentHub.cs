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
    public class CommentHub : Hub
    {
        private readonly IRepository<Rate> _repository;
        private readonly ILotRepository _lotRepository;

        public CommentHub(IRepository<Rate> repository, ILotRepository lotRepository)
        {
            _repository = repository;
            _lotRepository = lotRepository;
        }

        [Authorize]
        public async Task AddBet(string lotId, string bet, string culture)
        {
            var isDecimal = decimal.TryParse(bet, NumberStyles.Currency, new CultureInfo(culture), out var betM);
            var isInt = int.TryParse(lotId, out var id);

            if (isDecimal && isInt)
            {
                var rate = await SaveRate(id, betM);
                if (rate != null)
                {
                    await Clients.Group(lotId).SendAsync("UpdateBet", rate);
                }
                else
                {
                    await Clients.Caller.SendAsync("Exception", "The object has not preserved");
                }
            }
            else
            {
                await Clients.Caller.SendAsync("Exception", "Bet or lotId isn't correct");
            }
        }

        public Task JoinRoom(string lotId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, lotId);
        }

        public Task LeaveRoom(string lotId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, lotId);
        }

        private async Task<Rate> SaveRate(int lotId, decimal bet)
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