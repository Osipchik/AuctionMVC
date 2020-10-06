using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Auction.Models;

namespace Auction.Services
{
    public interface ILotRepository : IRepository<Lot>
    {
        ValueTask<int> Count();
        Task<Lot> FindUserLot(int lotId, string userId);
        Task<List<Lot>> FindRange(int take, int skip);
        Task<List<Lot>> FindRange(int take, int skip, Expression<Func<Lot, bool>> expression);
    }
}