using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostgreSqlProvider;
using Znaker.Models;

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

        public IActionResult Contact(long id)
        {
            var contact = _db.Contacts
                .Include(c => c.EntryContacts)
                .ThenInclude(ec => ec.Entry)
                .FirstOrDefault(c => c.Id == id);
            if (contact == null)
            {
                return NotFound();
            }
            return View(new ContactModel
            {
                Identity = contact.Identity,
                Text = contact.EntryContacts.Select(c => c.Entry.Text).ToList()
            });
        }
    }
}
