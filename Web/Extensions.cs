using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Web
{
    public static class Extensions
    {
        public static string UserId(this HttpContext context)
        {
            return context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}