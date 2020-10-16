using System.ComponentModel.DataAnnotations;

namespace Data.SortOptions
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