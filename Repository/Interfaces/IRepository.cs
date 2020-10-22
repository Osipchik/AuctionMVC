using System.Threading.Tasks;
using Data;

namespace Repository.Interfaces
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