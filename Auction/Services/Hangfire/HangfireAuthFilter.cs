using Hangfire.Dashboard;

namespace Auction.Services.Hangfire
{
    public class HangfireAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httContext = context.GetHttpContext();

            return httContext.User.Identity.IsAuthenticated;
        }
    }
}