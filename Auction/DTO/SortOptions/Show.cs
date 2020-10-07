using System.ComponentModel.DataAnnotations;

namespace Auction.DTO.SortOptions
{
    public enum Show
    {
        [Display(Name="Active")]
        Active,
        [Display(Name="Sold")]
        Sold,
        [Display(Name="All")]
        All
    }
}