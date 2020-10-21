using System.Collections.Generic;
using Data;
using Web.DTO.Pagination;

namespace Web.DTO
{
    public class CommentsView
    {
        public PagingInfo PagingInfo { get; set; }
        public List<Comment> Comments { get; set; }
    }
}