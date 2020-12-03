using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Core;

namespace Domain.Interfaces
{
    public interface ILotRepository<TSort, TShow>: IRepository<Lot>
    {
        Task<Lot> Find(int lotId, string userId, bool isAdmin, int timezoneOffset = 0);
        Task<IEnumerable<Lot>> FindRange(IQueryable<Lot> queryable, int take, int skip, int timezoneOffset = 0);
        IQueryable<Lot> FilterLots(TSort sortBy, TShow show, int categoryId);
        Task LoadRates(Lot lot);
    }
}