using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int status)
        {
            switch (status)
            {
                case 404:
                    ViewBag.Message = "Resource not found";
                    break;
                default:
                    ViewBag.Message = "Client Error";
                    break;
            }
            
            return View("ErrorPage");
        }
    }
}