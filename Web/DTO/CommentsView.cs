using System.Collections.Generic;
using Data;
using Web.DTO.Pagination;

namespace Web.DTO
{
    public class CommentsView
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public int LotId { get; set; }
        public List<Comment> Comments { get; set; }
    }
}