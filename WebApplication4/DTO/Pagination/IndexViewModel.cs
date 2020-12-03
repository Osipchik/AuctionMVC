using Infrastructure.Data.SortOptions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication4.DTO.Pagination
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