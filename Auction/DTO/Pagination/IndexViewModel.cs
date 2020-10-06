using System.Collections.Generic;
using Auction.Models;

namespace Auction.DTO.Pagination
{
    public class IndexViewModel
    {
        public IEnumerable<Lot> Lots { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}