using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Repository
{
    public static class Extensions
    {
        public static string UserId(this HttpContext context)
        {
            return context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}