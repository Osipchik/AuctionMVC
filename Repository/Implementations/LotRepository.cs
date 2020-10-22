using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Repository.SortOptions;

namespace Repository.Implementations
{
    public class LotRepository : Repository<Lot>, ILotRepository
    {
        public LotRepository(AppDbContext context) : base(context) { }


        public async ValueTask<int> GetTotalCount()
        {
            return await Context.Lots.CountAsync();
        }

        public async Task<Lot> Find(int lotId, HttpContext context)
        {
            var lot = await Find(lotId);
            var isValid = lot?.AppUserId == context.UserId() ||
                          context.User.IsInRole(Constants.AdminRole);
            
            return isValid ? lot : null;
        }

        public async Task<List<Lot>> FindRange(IQueryable<Lot> queryable, int take, int skip)
        {
            return await queryable.Skip(skip).Take(take).Include(i => i.Rates).ToListAsync();
        }

        private IQueryable<Lot> Order(SortBy sortBy, Expression<Func<Lot, bool>> expression)
        {
            var query = Context.Lots.Where(expression);
            query = sortBy switch
            {
                SortBy.Date => query.OrderBy(i => i.EndAt),
                SortBy.DistinctDate => query.OrderByDescending(i => i.EndAt),
                SortBy.Name => query.OrderBy(i => i.Title),
                SortBy.DistinctName => query.OrderByDescending(i => i.Title),
                SortBy.Goal => query.OrderBy(i => i.Goal),
                SortBy.Funded => query.Include(x => x.Rates)
                    .OrderBy(i => i.Rates.OrderByDescending(c => c.CreatedAt).FirstOrDefault().Amount),
                _ => query
            };

            return query;
        }
        
        public IQueryable<Lot> FilterLots(SortBy sortBy, ShowOptions show, HttpContext context)
        {
            var query = show switch
            {
                ShowOptions.Active => Order(sortBy, i => i.IsAvailable && i.EndAt > DateTime.UtcNow),
                ShowOptions.Sold => Order(sortBy, i => i.IsAvailable && i.EndAt < DateTime.UtcNow),
                ShowOptions.All => Order(sortBy, i => i.IsAvailable),
                ShowOptions.MyLots => context.User.Identity.IsAuthenticated 
                    ? Order(sortBy, i => i.AppUserId == context.UserId())
                    : Order(sortBy, i => i.IsAvailable)
            };
            
            return query;
        }

        public async Task LoadRates(Lot lot)
        {
            await Context.Entry(lot).Collection(i => i.Rates).LoadAsync();
        }
    }
}