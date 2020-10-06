using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Auction.Data;
using Auction.Models;
using Microsoft.EntityFrameworkCore;

namespace Auction.Services
{
    public class LotRepository : ILotRepository
    {
        private readonly AppDbContext _context;

        public LotRepository(AppDbContext context)
        {
            _context = context;
        }

        public async ValueTask<int> Count() =>
            await _context.Lots.CountAsync();

        public async Task<Lot> Add(Lot entity)
        {
            await _context.Lots.AddAsync(entity);
            await _context.SaveChangesAsync();
            
            return entity;
        }

        public async Task<Lot> Update(Lot entity)
        {
            _context.Lots.Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<Lot> Delete(Lot entity)
        {
            _context.Lots.Remove(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<Lot> Find(int id)
        {
            return await _context.Lots.FindAsync(id);
        }

        public async Task<Lot> FindUserLot(int lotId, string userId)
        {
            return await _context.Lots.SingleOrDefaultAsync(i => i.Id == lotId && i.AppUserId == userId);
        }

        public async Task<List<Lot>> FindRange(int take, int skip)
        {
            return await _context.Lots.Skip(skip).Take(take).ToListAsync();
        }

        public async Task<List<Lot>> FindRange(int take, int skip, Expression<Func<Lot, bool>> expression)
        {
            return  await _context.Lots.Where(expression).Skip(skip).Take(take).ToListAsync();
        }
    }
}