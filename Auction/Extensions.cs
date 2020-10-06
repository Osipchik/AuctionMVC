using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Auction
{
    public static class Extensions
    {
        private const string UserIdClaimType = "sub";
        
        public static string UserId(this HttpContext context)
        {
            return context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}