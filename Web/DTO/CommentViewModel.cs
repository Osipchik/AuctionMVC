using System;
using Data;

namespace Web.DTO
{
    public class CommentViewModel
    {
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public int LotId { get; set; }
        public string UserName { get; set; }
    }
}