using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace WebApplication4
{
    public class DbInit
    {
        public static async Task InitialRoles(IConfiguration configuration, RoleManager<IdentityRole> roleManager)
        {
            var roles = configuration.GetRoles();
            foreach (var i in roles)
            {
                if (await roleManager.FindByNameAsync(i) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole(i));
                }
            }
        }
    }
}