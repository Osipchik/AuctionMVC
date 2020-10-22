using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Repository.Interfaces
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<List<Comment>> GetCommentsList(int lotId, int take, int skip);
    }
}