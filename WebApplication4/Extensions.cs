using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace WebApplication4
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

        public static int GetTimezoneOffset(this Controller controller)
        {
            var offset = 0;
            if (int.TryParse(controller.HttpContext.Request.Cookies["timezoneOffset"], out var timezoneOffset))
            {
                offset = timezoneOffset;
            }

            return offset;
        }
    }
}