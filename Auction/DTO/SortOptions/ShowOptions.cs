using System.ComponentModel.DataAnnotations;

namespace Auction.DTO.SortOptions
{
    public enum ShowOptions
    {
        [Display(Name="Active")]
        Active,
        [Display(Name="Sold")]
        Sold,
        [Display(Name="All")]
        All,
        [Display(Name="My Lots")]
        MyLots
    }
}