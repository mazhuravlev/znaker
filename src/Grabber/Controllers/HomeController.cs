using System.Linq;
using System.Threading.Tasks;
using Grabber.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostgreSqlProvider;

namespace Grabber.Controllers
{
    public class HomeController : Controller
    {
        private IProxyService _proxyService;

        public HomeController(IProxyService proxyService)
        {
            _proxyService = proxyService;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}