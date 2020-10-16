using System.Collections.Generic;
using Auction.DTO.SortOptions;
using Auction.Models;

namespace Auction.DTO.Pagination
{
    public class IndexViewModel
    {
        public List<LotPreview> Lots { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public ShowOptions ShowOptions { get; set; }
        public SortBy SortBy { get; set; }
        public string Filter { get; set; }
    }
}