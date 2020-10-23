using System.Collections.Generic;
using Repository.SortOptions;
using Web.DTO.Lot;

namespace Web.DTO.Pagination
{
    public class IndexViewModel
    {
        public List<LotPreview> Lots { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public ShowOptions ShowOptions { get; set; }
        public SortBy SortBy { get; set; }
        public string Search { get; set; }
    }
}