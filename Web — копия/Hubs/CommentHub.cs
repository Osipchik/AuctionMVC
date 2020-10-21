using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Web.DTO;

namespace Web.Hubs
{
    [Authorize]
    public class CommentHub : Hub
    {
        public async Task PostComment(CommentMessage commentMessage)
        {
            await Clients.Group(commentMessage.LotId).SendAsync("ReceiveComment", commentMessage.Message);
        }

        public Task JoinRoom(string lotId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, lotId);
        }

        public Task LeaveRoom(string lotId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, lotId);
        }
    }
}