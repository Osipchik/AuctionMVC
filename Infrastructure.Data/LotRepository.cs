using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Core;
using Microsoft.EntityFrameworkCore;
using Domain.Interfaces;
using Infrastructure.Data.SortOptions;

namespace Infrastructure.Data
{
    public class LotRepository : Repository<Lot>, ILotRepository<SortBy, ShowOptions>
    {
        public LotRepository(AppDbContext context) : base(context) { }

        public async Task<Lot> Find(int lotId, string userId, bool isAdmin)
        {
            var lot = await Find(lotId);
            var isValid = lot?.AppUserId == userId || isAdmin;
            
            return isValid ? lot : null;
        }

        public async Task<IEnumerable<Lot>> FindRange(IQueryable<Lot> queryable, int take, int skip)
        {
            return await queryable.Skip(skip).Take(take).ToListAsync();
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
                SortBy.Goal => query.OrderBy(i => i.MinPrice),
                SortBy.Funded => query.OrderBy(i => i.Rates.OrderByDescending(c => c.CreatedAt)
                    .FirstOrDefault().Amount),
                _ => query
            };
            
            return query;
        }
        
        public IQueryable<Lot> FilterLots(SortBy sortBy, ShowOptions show, int categoryId)
        {
            IQueryable<Lot> query;
            
            if (categoryId != 0)
            {
                query = show switch
                {
                    ShowOptions.Active => Order(sortBy, i => i.IsAvailable && i.EndAt > DateTime.UtcNow && i.CategoryId == categoryId),
                    ShowOptions.Sold => Order(sortBy, i => i.IsAvailable && i.EndAt < DateTime.UtcNow && i.CategoryId == categoryId),
                    ShowOptions.All => Order(sortBy, i => i.IsAvailable && i.CategoryId == categoryId),
                    _ => Order(sortBy,  i => i.CategoryId == categoryId)
                };
            }
            else
            {
                query = show switch
                {
                    ShowOptions.Active => Order(sortBy, i => i.IsAvailable && i.EndAt > DateTime.UtcNow),
                    ShowOptions.Sold => Order(sortBy, i => i.IsAvailable && i.EndAt < DateTime.UtcNow),
                    ShowOptions.All => Order(sortBy, i => i.IsAvailable),
                    _ => Order(sortBy)
                };   
            }

            return query;
        }

        public async Task LoadRates(Lot lot)
        {
            await Context.Entry(lot).Collection(i => i.Rates).LoadAsync();
        }

        // private static Lot SelectLotView(Lot lot)
        // {
        //     var lotView = new Lot(lot);
        //
        //     return lotView;
        // }
    }
}