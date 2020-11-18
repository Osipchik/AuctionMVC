using System;
using System.Linq;
using Domain.Core;

namespace Infrastructure.Data
{
    public class LotView
    {
        public int Id { get; set; }
        
        public string ImageUrl { get; set; }
        
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime LunchAt { get; set; }
        
        public DateTime EndAt { get; set; }
        
        public decimal MinPrice { get; set; }
        public decimal CurrentBet { get; set; }
        
        public int BetCount { get; set; }
        
        public Category Category { get; set; }

        public string AppUserId { get; set; }
        public string UserName { get; set; }

        public bool IsAvailable { get; set; }

        public LotView(Lot lot)
        {
            Id = lot.Id;
            ImageUrl = lot.ImageUrl;
            Title = lot.Title;
            Description = lot.Description;
            LunchAt = lot.LunchAt;
            EndAt = lot.EndAt;
            MinPrice = lot.MinPrice;
            CurrentBet = lot.Rates?.OrderByDescending(c => c.CreatedAt).FirstOrDefault()?.Amount ?? 0m;
            BetCount = lot.Rates?.Count ?? 0;
            Category = lot.Category;
            AppUserId = lot.AppUserId;
            IsAvailable = lot.IsAvailable;
        }
    }
}