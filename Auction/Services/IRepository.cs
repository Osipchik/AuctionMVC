using System.Threading.Tasks;
using Auction.Models;

namespace Auction.Services
{
    public interface IRepository<TEntity>
        where TEntity : Entity
    {

        Task<TEntity> Add(TEntity entity);

        Task<TEntity> Update(TEntity entity);

        Task<TEntity> Delete(TEntity entity);
        
        Task<TEntity> Find(int id);
    }
}