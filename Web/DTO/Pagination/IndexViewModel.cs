using System.Collections.Generic;
using Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repository;
using Repository.SortOptions;
using Web.DTO.Lot;

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