using Hangfire.Dashboard;

namespace WebApplication4
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