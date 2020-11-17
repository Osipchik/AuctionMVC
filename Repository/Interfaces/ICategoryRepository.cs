using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Repository.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetAll();
    }
}