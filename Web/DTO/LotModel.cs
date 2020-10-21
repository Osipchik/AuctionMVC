using System;
using System.ComponentModel.DataAnnotations;
using Data;

namespace Web.DTO
{
    public class LotModel
    {
        [RegularExpression(@"^((?!@).)*$", ErrorMessage = "Title can't contain '@'.")]
        [MaxLength(35, ErrorMessage = "Title must be less then 35 characters")]
        public string Title { get; set; }
        
        [MaxLength(135, ErrorMessage = "Description must be less then 35 characters")]
        public string Description { get; set; }
        
        [DataType(DataType.Url)]
        public string ImageUrl { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime LunchAt { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime EndAt { get; set; }
        
        [RegularExpression("^[0-9]+", ErrorMessage = "It must be number")]
        public decimal Goal { get; set; }
        
        [DataType(DataType.MultilineText)]
        public string Story { get; set; }

        public LotModel() {}
        
        public LotModel(Lot lot)
        {
            Title = lot.Title;
            Description = lot.Description;
            ImageUrl = lot.ImageUrl;
            LunchAt = lot.LunchAt;
            EndAt = lot.EndAt;
            Goal = lot.Goal;
            Story = lot.Story;
        }
    }
}