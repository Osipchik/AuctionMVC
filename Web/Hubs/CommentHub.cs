using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs
{
    [Authorize]
    public class CommentHub : Hub
    {
        public async Task PostComment(string message)
        {
            await Clients.All.SendAsync("ReceiveComment");
        }
    }
}