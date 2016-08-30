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
        private readonly ZnakerContext _db;

        public HomeController(ZnakerContext db)
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
            var contact = await _db.Contacts
                .Include(c => c.EntryContacts)
                .ThenInclude(ec => ec.Entry)
                .FirstOrDefaultAsync(c => c.Identity == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(new ContactModel
            {
                Identity = contact.Identity,
                Entries = contact.EntryContacts.Select(ec => new EntryModel
                {
                    Id = ec.EntryId,
                    Contact = ec.Contact.Identity,
                    Text = ec.Entry.Text
                }).ToList()
            });
        }

        [Route("{contact}/entry/{id}")]
        public IActionResult Entry(string contact, int id)
        {
            var entry = _db.Entries.Include(e => e.Source).First(e => e.Id == id);
            if (null == entry)
            {
                return NotFound();
            }
            var entryModel = new EntryModel
            {
                Text = entry.Text,
                Contact = contact,
                Source = entry.Source.Title,
                CreatedOn = entry.CreatedOn
            };
            return View(entryModel);
        }
    }
}