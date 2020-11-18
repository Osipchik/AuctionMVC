using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Data.SortOptions
{
    public enum SortBy
    {
        [Display(Name = "↓Date")]
        Date,
        [Display(Name = "↑Date")]
        DistinctDate,
        [Display(Name = "↑Name")]
        Name,
        [Display(Name = "↓Name")]
        DistinctName,
        Goal,
        Funded 
    }
}