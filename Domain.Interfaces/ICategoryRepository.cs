using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Core;

namespace Domain.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetAll();
    }
}