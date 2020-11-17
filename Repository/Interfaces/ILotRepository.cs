using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Repository.SortOptions;

namespace Repository.Interfaces
{
    public interface ILotRepository : IRepository<Lot>
    {
        Task<Lot> Find(int lotId, string userId, bool isAdmin);
        Task<IEnumerable<LotView>> FindRange(IQueryable<Lot> queryable, int take, int skip);
        IQueryable<Lot> FilterLots(SortBy sortBy, ShowOptions show, int categoryId);
        Task LoadRates(Lot lot);
    }
}