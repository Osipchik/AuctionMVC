using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Web
{
    public static class Extensions
    {
        public static string UserId(this HttpContext context)
        {
            return context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        
        public static List<string> GetRoles(this IConfiguration configuration)
        {
            return configuration.GetSection("Roles").GetChildren().ToArray().Select(c => c.Value).ToList();
        }
    }
}