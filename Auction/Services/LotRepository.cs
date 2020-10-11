using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Auction.Data;
using Auction.DTO;
using Auction.DTO.SortOptions;
using Auction.Models;
using Microsoft.EntityFrameworkCore;

namespace Auction.Services
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

        public async Task<List<LotPreview>> FindRange(int take, int skip, Expression<Func<Lot, bool>> expression)
        {
            return await FindRange(Context.Lots.Where(expression), take, skip);
        }

        public async Task<List<LotPreview>> Order(int take, int skip, SortBy sortBy, Expression<Func<Lot, bool>> expression)
        {
            var query = Context.Lots.Where(expression);
            query = sortBy switch
            {
                SortBy.Date => query.OrderBy(i => i.LunchAt),
                SortBy.DistinctDate => query.OrderByDescending(i => i.LunchAt),
                SortBy.Name => query.OrderBy(i => i.Title),
                SortBy.DistinctName => query.OrderByDescending(i => i.Title),
                SortBy.Goal => query.OrderBy(i => i.Goal),
                SortBy.Funded => query.OrderBy(i => i.Funded),
                _ => query
            };

            return await FindRange(query, take, skip);
        }
    }
}