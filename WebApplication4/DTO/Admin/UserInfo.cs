using System.Collections.Generic;
using Domain.Core;
using Microsoft.AspNetCore.Identity;

namespace WebApplication4.DTO.Admin
{
    public class UserInfo
    {
        public AppUser User { get; set; }
        public IEnumerable<IdentityRole> Roles { get; set; }
        public IEnumerable<string> UserRoles { get; set; }
    }
}