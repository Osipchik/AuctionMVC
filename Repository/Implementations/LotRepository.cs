using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Data;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Repository.SortOptions;

namespace Repository.Implementations
{
    public class LotRepository : Repository<Lot>, ILotRepository
    {
        public LotRepository(AppDbContext context) : base(context) { }

        public async Task<Lot> Find(int lotId, string userId, bool isAdmin)
        {
            var lot = await Find(lotId);
            var isValid = lot?.AppUserId == userId || isAdmin;
            
            return isValid ? lot : null;
        }

        public async Task<IEnumerable<LotView>> FindRange(IQueryable<Lot> queryable, int take, int skip)
        {
            return queryable.Select(i => SelectLotView(i)).Skip(skip).Take(take).AsEnumerable();
        }

        private IQueryable<Lot> Order(SortBy sortBy, Expression<Func<Lot, bool>> expression = null)
        {
            var query = Context.Lots.Include(x => x.Rates).AsNoTracking().AsQueryable();

            if (expression != null)
            {
                query = query.Where(expression);
            }
            
            query = sortBy switch
            {
                SortBy.Date => query.OrderBy(i => i.EndAt),
                SortBy.DistinctDate => query.OrderByDescending(i => i.EndAt),
                SortBy.Name => query.OrderBy(i => i.Title),
                SortBy.DistinctName => query.OrderByDescending(i => i.Title),
                SortBy.Goal => query.OrderBy(i => i.Goal),
                SortBy.Funded => query.OrderBy(i => i.Rates.OrderByDescending(c => c.CreatedAt)
                    .FirstOrDefault().Amount),
                _ => query
            };
            
            return query;
        }
        
        public IQueryable<Lot> FilterLots(SortBy sortBy, ShowOptions show)
        {
            var query = show switch
            {
                ShowOptions.Active => Order(sortBy, i => i.IsAvailable && i.EndAt > DateTime.UtcNow),
                ShowOptions.Sold => Order(sortBy, i => i.IsAvailable && i.EndAt < DateTime.UtcNow),
                ShowOptions.All => Order(sortBy, i => i.IsAvailable),
                _ => Order(sortBy)
            };
            
            return query;
        }

        public async Task LoadRates(Lot lot)
        {
            await Context.Entry(lot).Collection(i => i.Rates).LoadAsync();
        }

        private static LotView SelectLotView(Lot lot)
        {
            var lotView = new LotView(lot);

            return lotView;
        }
    }
}