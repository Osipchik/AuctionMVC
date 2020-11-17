using System.Collections;
using System.Collections.Generic;
using Data;

namespace Web.DTO.Admin
{
    public class PanelViewModel
    {
        public IEnumerable<AppUser> UserViewModels { get; set; }
        public IEnumerable<string> Roles { get; set; }
        
    }
}