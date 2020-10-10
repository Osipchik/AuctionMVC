using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Auction.Data;
using Auction.DTO;
using Auction.DTO.SortOptions;
using Auction.Models;

namespace Auction.Services
{
    public interface ILotRepository : IRepository<Lot>
    {
        AppDbContext Context { get; set; }
        ValueTask<int> Count();
        Task<Lot> FindUserLot(int lotId, string userId);
        Task<List<LotPreview>> FindRange(int take, int skip, Expression<Func<Lot, bool>> expression);
        Task<List<LotPreview>> Order(int take, int skip, SortBy sortBy, Expression<Func<Lot, bool>> expression);
    }
}