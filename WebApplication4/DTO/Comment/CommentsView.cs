using System.Collections.Generic;

namespace WebApplication4.DTO.Comment
{
    public class CommentsView
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public int LotId { get; set; }
        public List<Domain.Core.Comment> Comments { get; set; }
    }
}