using Domain.Core;
using System;

namespace WebApplication4.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public AppUser User { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
