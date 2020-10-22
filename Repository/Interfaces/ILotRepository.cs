using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Http;
using Repository.SortOptions;

namespace Repository.Interfaces
{
    public interface ILotRepository : IRepository<Lot>
    {
        ValueTask<int> GetTotalCount();
        Task<Lot> Find(int lotId, HttpContext context);
        Task<List<Lot>> FindRange(IQueryable<Lot> queryable, int take, int skip);
        IQueryable<Lot> FilterLots(SortBy sortBy, ShowOptions show, HttpContext context);
        Task LoadRates(Lot lot);
    }
}