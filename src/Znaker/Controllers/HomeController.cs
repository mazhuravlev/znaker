using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostgreSqlProvider;
using Znaker.Models;
using Sakura.AspNetCore;

namespace Znaker.Controllers
{
    public class HomeController : Controller
    {
        private readonly PostgreSqlContext _db;

        public HomeController(PostgreSqlContext db)
        {
            _db = db;
        }

        public IActionResult Index(int page = 1)
        {
            var m = new HomeModel
            {
                TotalEntries = _db.Entries.Count(),
                Items = _db.Contacts
                    .Include(c => c.EntryContacts)
                    .ThenInclude(c => c.Entry)
                    .Select(c => new HomeModel.Item
                    {
                        Identity = c.Identity,
                        Data = c.EntryContacts.Select(z => z.Entry.Text).FirstOrDefault(),
                        TotalData = c.EntryContacts.Count
                    }).ToPagedList(100, page)
            };
            return View(m);
        }
        [Route("{id}")]
        public async Task<IActionResult> Contact(string id)
        {
            var contact =  await _db.Contacts.FirstOrDefaultAsync(c => c.Identity == id);
            if (contact == null)
            {
                //или редирект на страницу ненаденного контакта
                return NotFound();
            }


            return View(new ContactModel
            {
                Identity = contact.Identity
            });
        }
    }
}
