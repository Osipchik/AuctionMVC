using System.Collections.Generic;
using Domain.Core;

namespace WebApplication4.DTO.Admin
{
    public class PanelViewModel
    {
        public IEnumerable<AppUser> UserViewModels { get; set; }
        public IEnumerable<string> Roles { get; set; }
        
    }
}