using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Core;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace UnitTest
{
    public static class Utilities
    {
        public static async Task Initialize(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context)
        {
            await InitialRoles(roleManager);
            var user = await InitialUser(userManager);
            await InitialCategories(context);
            await InitialLots(context, user.Id);
            
            await InitialComments(context, 1, user.Id);
            await InitialRates(context, 1, user.Id);
            
            await InitialComments(context, 2, user.Id);
            await InitialRates(context, 2, user.Id);
        }
        
        private static async Task InitialRoles(RoleManager<IdentityRole> roleManager)
        {
            var roles = new List<string>{"superAdmin", "admin", "user"};
            foreach (var i in roles)
            {
                if (await roleManager.FindByNameAsync(i) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole(i));
                }
            }
        }

        private static async Task<AppUser> InitialUser(UserManager<AppUser> userManager)
        {
            var roles = new List<string>{"superAdmin", "admin", "user"};
            var adminConfig = new Dictionary<string, string> {{"Email", "test@gmail.com"}};
            
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
                
                return admin;
            }
            else
            {
                throw new Exception("user wasn't created");
            }
        }

        private static async Task InitialCategories(AppDbContext context)
        {
            var categories = new List<Category>();
            for (var i = 0; i < 5; i++)
            {
                categories.Add(new Category
                {
                    Name = "category_" + i
                });
            }
            
            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        private static async Task InitialLots(AppDbContext context, string userId)
        {
            var lots = new List<Lot>();

            for (var i = 0; i < 5; i++)
            {
                lots.Add(new Lot
                {
                    AppUserId = userId,
                    CategoryId = 1,
                    EndAt = DateTime.Now.AddDays(1),
                    LunchAt = DateTime.Now.AddSeconds(20),
                    CreatedAt = DateTime.Now.AddSeconds(20),
                    Title = "test_lot_title: true - " + i,
                    IsAvailable = i < 4
                });
            }
            
            for (var i = 5; i < 10; i++)
            {
                lots.Add(new Lot
                {
                    AppUserId = userId,
                    CategoryId = 2,
                    EndAt = DateTime.Now.Subtract(TimeSpan.FromDays(2)),
                    LunchAt = DateTime.Now.Subtract(TimeSpan.FromDays(6)),
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(7)),
                    Title = "test_lot_title: false - " + i,
                    IsAvailable = true
                });
            }
            
            for (var i = 10; i < 15; i++)
            {
                lots.Add(new Lot
                {
                    AppUserId = userId,
                    EndAt = DateTime.Now.AddDays(1),
                    LunchAt = DateTime.Now.AddSeconds(20),
                    CreatedAt = DateTime.Now.AddSeconds(20),
                    Title = "test_lot_title: false - " + i,
                    IsAvailable = false
                });
            }

            await context.Lots.AddRangeAsync(lots);
            await context.SaveChangesAsync();
        }

        private static async Task InitialComments(AppDbContext context, int lotId, string userId)
        {
            var comments = new List<Comment>();
            for (var i = 0; i < 20; i++)
            {
                comments.Add(new Comment
                {
                    AppUserId = userId,
                    LotId = lotId
                });
            }
            
            await context.Comments.AddRangeAsync(comments);
            await context.SaveChangesAsync();
        }

        private static async Task InitialRates(AppDbContext context, int lotId, string userId)
        {
            var rates = new List<Rate>();
            for (var i = 0; i < 20; i++)
            {
                rates.Add(new Rate
                {
                    AppUserId = userId,
                    LotId = lotId,
                    CreatedAt = DateTime.Now,
                    Amount = 10 * i + 1
                });
            }
            
            await context.Rates.AddRangeAsync(rates);
            await context.SaveChangesAsync();
        }
    }
}