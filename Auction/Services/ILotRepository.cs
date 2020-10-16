using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Auction.Data;
using Auction.DTO;
using Auction.DTO.SortOptions;
using Auction.Models;
using Microsoft.AspNetCore.Http;

namespace Auction.Services
{
    public interface ILotRepository : IRepository<Lot>
    {
        AppDbContext Context { get; set; }
        ValueTask<int> Count();
        Task<Lot> Find(int lotId, HttpContext context);
        Task<List<LotPreview>> FindRange(IQueryable<Lot> queryable, int take, int skip);
        IQueryable<Lot> FilterLots(SortBy sortBy, ShowOptions show, HttpContext context);
    }
}