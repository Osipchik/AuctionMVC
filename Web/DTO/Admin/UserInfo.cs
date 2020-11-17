using System.Collections;
using System.Collections.Generic;
using Data;
using Microsoft.AspNetCore.Identity;
using Web.DTO.Lot;

namespace Web.DTO.Admin
{
    public class UserInfo
    {
        public AppUser User { get; set; }
        public IEnumerable<IdentityRole> Roles { get; set; }
        public IEnumerable<string> UserRoles { get; set; }
    }
}