using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository;
using Repository.Interfaces;
using Web.DTO;
using Web.DTO.Comment;

namespace Web.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentRepository _repository;
        private readonly IRepository<Lot> _lotRepository;

        public CommentController(ICommentRepository repository, IMapper mapper, IRepository<Lot> lotRepository)
        {
            _repository = repository;
            _lotRepository = lotRepository;
        }
        

        [HttpGet]
        public async Task<IActionResult> GetComments(int lotId, int take, int skip)
        {
            var comments = await _repository.GetCommentsList(lotId, take, skip);

            if (comments.Count != 0)
            {
                // var commentsVm = comments.Select(_mapper.Map<Comment, CommentViewModel>).ToList();
                var commentView = new CommentsView
                {
                    Comments = comments,
                    Take = take,
                    Skip = skip,
                    LotId = lotId
                };
                
                return PartialView("_Comments", commentView);
            }

            return NotFound();
        }

        [HttpDelete]
        [Authorize(Roles = Constants.AdminRole)]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var comment = await _repository.Find(commentId);

            if (comment != null)
            {
                await _repository.Delete(comment);
                return Ok();
            }

            return NotFound();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostComment(int lotId, string message)
        {
            var lot = await _lotRepository.Find(lotId);
            if (lot != null && !string.IsNullOrWhiteSpace(message))
            {
                var comment = new Comment
                {
                    AppUserId = HttpContext.UserId(),
                    CreatedAt = DateTime.UtcNow,
                    LotId = lotId,
                    Text = message
                };

                await _repository.Add(comment);
                
                var commentView = new CommentsView
                {
                    Comments = await _repository.GetCommentsList(lotId, 10, 0),
                    Take = 10,
                    Skip = 0,
                    LotId = lotId
                };

                return PartialView("_Comments", commentView);
            }

            return BadRequest();
        }
    }
}