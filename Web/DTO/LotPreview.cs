using System;

namespace Web.DTO
{
    public class LotPreview
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public DateTime LunchAt { get; set; }
        public DateTime EndAt { get; set; }
        public decimal Goal { get; set; }
        public decimal Funded { get; set; }
    }
}