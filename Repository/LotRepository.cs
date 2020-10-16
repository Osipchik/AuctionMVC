using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Data;
using Data.DTO;
using Data.SortOptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class LotRepository : ILotRepository
    {
        // public readonly AppDbContext Context;
        public AppDbContext Context { get; set; }

        public LotRepository(AppDbContext context)
        {
            Context = context;
        }

        public async ValueTask<int> Count() =>
            await Context.Lots.CountAsync();

        public async Task<Lot> Add(Lot entity)
        {
            await Context.Lots.AddAsync(entity);
            await Context.SaveChangesAsync();
            
            return entity;
        }

        public async Task<Lot> Update(Lot entity)
        {
            Context.Lots.Update(entity);
            await Context.SaveChangesAsync();

            return entity;
        }

        public async Task<Lot> Delete(Lot entity)
        {
            Context.Lots.Remove(entity);
            await Context.SaveChangesAsync();

            return entity;
        }

        public async Task<Lot> Find(int lotId, HttpContext context)
        {
            var lot = await Find(lotId);
            var isValid = lot?.AppUserId == context.UserId() ||
                          context.User.IsInRole(Constants.AdminRole);
            
            return isValid ? lot : null;
        }
        
        public async Task<Lot> Find(int id)
        {
            return await Context.Lots.FindAsync(id);
        }

        public async Task<Lot> FindUserLot(int lotId, string userId)
        {
            return await Context.Lots.SingleOrDefaultAsync(i => i.Id == lotId && i.AppUserId == userId);
        }

        public async Task<List<LotPreview>> FindRange(IQueryable<Lot> queryable, int take, int skip)
        {
            return await queryable.Skip(skip).Take(take).Include(i => i.Rates)
                .Select(i => new LotPreview
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    ImageUrl = i.ImageUrl,
                    LunchAt = i.LunchAt,
                    EndAt = i.EndAt,
                    Goal = i.Goal,
                    Funded = i.Rates.OrderByDescending(c => c.CreatedAt).FirstOrDefault().Amount
                }).ToListAsync();
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
                SortBy.Funded => query.OrderBy(i => i.Funded),
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
    }
}