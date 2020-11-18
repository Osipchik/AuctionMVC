using Microsoft.AspNetCore.Mvc.Rendering;
using Infrastructure.Data.SortOptions;

namespace Web.DTO.Pagination
{
    public class IndexViewModel
    {
        public SelectList Categories { get; set; }
        public ShowOptions ShowOptions { get; set; }
        public SortBy SortBy { get; set; }
        public string Search { get; set; }
        public int CategoryId { get; set; }
    }
}