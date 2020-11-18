using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Domain.Core
{
    public class AppUser : IdentityUser
    {
        public List<Lot> Lots { get; set; }
        public List<Rate> Rates { get; set; }
    }
}