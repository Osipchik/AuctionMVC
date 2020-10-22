using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Interfaces;
using Web.DTO;

namespace Web.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentRepository _repository;
        private readonly IMapper _mapper;

        public CommentController(ICommentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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

            return NoContent();
        }

        [HttpDelete]
        [Authorize]
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
    }
}