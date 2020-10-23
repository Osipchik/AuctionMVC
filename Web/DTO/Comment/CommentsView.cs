using System.Collections.Generic;

namespace Web.DTO.Comment
{
    public class CommentsView
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public int LotId { get; set; }
        public List<Data.Comment> Comments { get; set; }
    }
}