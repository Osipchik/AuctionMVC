using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Mvc;
using Repository;
using Web.DTO;
using Web.DTO.Pagination;

namespace Web.Controllers
{
    public class CommentController : Controller
    {
        private int pageSize = 3;
        private readonly IRepository<Comment> _repository;

        public CommentController(IRepository<Comment> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetPage(int page = 1)
        {
            var model = new CommentsView
            {
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = 3
                },
                Comments = 
            };
            
            return PartialView("_Comments", model);
        }
    }
}