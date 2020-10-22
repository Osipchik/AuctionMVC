using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository.Implementations
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(AppDbContext context) : base(context) { }

        
        public async Task<List<Comment>> GetCommentsList(int lotId, int take, int skip)
        {
            return await Context.Comments
                .OrderByDescending(i => i.CreatedAt)
                .Skip(skip).Take(take)
                .Include(i => i.AppUser)
                .Where(i => i.LotId == lotId)
                .ToListAsync();
        }
    }
}