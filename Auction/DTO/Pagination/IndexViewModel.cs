using System.Collections.Generic;
using Auction.DTO.SortOptions;
using Auction.Models;

namespace Auction.DTO.Pagination
{
    public class IndexViewModel
    {
        public IEnumerable<Lot> Lots { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public Show Show { get; set; }
        public SortBy SortBy { get; set; }
        public string Filter { get; set; }
    }
}