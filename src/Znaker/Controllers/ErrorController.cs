using Microsoft.AspNetCore.Mvc;

namespace Znaker.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet("statuscode/{code}")]
        public IActionResult Index(int code)
        {
            ViewBag.Code = code;
            return View();
        }
    }
}
