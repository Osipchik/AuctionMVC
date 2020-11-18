using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Core;

namespace Domain.Interfaces
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<List<Comment>> GetCommentsList(int lotId, int take, int skip);
    }
}