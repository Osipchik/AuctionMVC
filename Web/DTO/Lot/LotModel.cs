using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Web.DTO.Lot
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
        [DisplayName("EndAt")]  
        public DateTime EndAt { get; set; }
        
        [Range(0, 1_000_000)]
        [DisplayName("Min price")]  
        public decimal MinPrice { get; set; }
        
        [DataType(DataType.MultilineText)]
        public string Story { get; set; }
        
        public int CategoryId { get; set; }
        
        public LotModel() {}
        
        public LotModel(Domain.Core.Lot lot)
        {
            Title = lot.Title;
            Description = lot.Description;
            ImageUrl = lot.ImageUrl;
            EndAt = lot.EndAt;
            CategoryId = lot.CategoryId;
            MinPrice = lot.MinPrice;
            Story = lot.Story;
        }
    }
}