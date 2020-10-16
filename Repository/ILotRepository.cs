using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.DTO;
using Data.SortOptions;
using Microsoft.AspNetCore.Http;

namespace Repository
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