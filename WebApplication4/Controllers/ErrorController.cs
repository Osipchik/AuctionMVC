using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace WebApplication4.Controllers
{
    public class ErrorController : Controller
    {
        private readonly IConfiguration _configuration;
        
        public ErrorController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        [Route("Error/{code}")]
        public IActionResult Error(int code)
        {
            var messages = _configuration.GetSection("ErrorMessages");
            
            ViewBag.Code = code;
            ViewBag.Message = code >= 500 ? messages["5xx"] : messages["4xx"];
            return View();
        }
    }
}