using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Core;
using Microsoft.EntityFrameworkCore;
using Domain.Interfaces;

namespace Infrastructure.Data
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Category>> GetAll()
        {
            return await Context.Categories.AsNoTracking().OrderBy(i => i.Name).ToListAsync();
        }
    }
}