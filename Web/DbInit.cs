using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Web
{
    public class DbInit
    {
        public static async Task Initialize(
            IConfiguration configuration, 
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            await InitialRoles(configuration, roleManager);
            await UserEnsureCreated(configuration, userManager);
        }
        
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

        private static async Task UserEnsureCreated(IConfiguration configuration, UserManager<AppUser> userManager)
        {
            var roles = configuration.GetRoles();
            var adminConfig = configuration.GetSection("Admin");
            
            if (!userManager.Users.Any())
            {
                var admin = new AppUser
                {
                    Email = adminConfig["Email"],
                    UserName = adminConfig["Email"],
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, adminConfig["password"]);
                if (result.Succeeded)
                {
                    await userManager.AddToRolesAsync(admin, new[] {roles[0], roles[1]});
                }
                else
                {
                    throw new Exception("user wasn't created");
                }
            }
        }

        // private static List<string> GetRoles(IConfiguration configuration)
        // {
        //     return configuration.GetSection("Roles").GetChildren().ToArray().Select(c => c.Value).ToList();
        // }
    }
}