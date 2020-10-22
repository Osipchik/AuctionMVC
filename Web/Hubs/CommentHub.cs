using System;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Repository;
using Repository.Interfaces;
using Web.DTO;

namespace Web.Hubs
{
    public class CommentHub : Hub
    {
        private readonly IRepository<Comment> _repository;
        private readonly UserManager<AppUser> _manager;

        public CommentHub(IRepository<Comment> repository, UserManager<AppUser> manager)
        {
            _repository = repository;
            _manager = manager;
        }


        [Authorize]
        public async Task PostComment(CommentMessage commentMessage)
        {
            var comment = await SaveComment(commentMessage);
            comment.AppUser = await _manager.FindByIdAsync(Context.UserIdentifier);
            
            await Clients.Group(commentMessage.LotId).SendAsync("ReceiveComment", comment);
        }

        public Task JoinRoom(string lotId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, lotId);
        }

        public Task LeaveRoom(string lotId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, lotId);
        }

        private async Task<Comment> SaveComment(CommentMessage commentMessage)
        {
            var comment = new Comment
            {
                AppUserId = Context.UserIdentifier,
                CreatedAt = DateTime.UtcNow,
                Text = commentMessage.Message,
                LotId = int.Parse(commentMessage.LotId)
            };

            return await _repository.Add(comment);
        }
    }
}