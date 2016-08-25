using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostgreSqlProvider;

namespace Znaker.Controllers
{
    public class HomeController : Controller
    {
        private readonly PostgreSqlContext _db;

        public HomeController(PostgreSqlContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            ViewBag.test = "Call to action!!!!!!!";
            return View();
        }

        public IActionResult Contact([FromRoute] int id)
        {
            var contact = _db.Contacts
                .Include(c => c.EntryContacts)
                .ThenInclude(ec => ec.Entry)
                .First(c => c.Id == id);
            return View(contact);
        }
    }
}
