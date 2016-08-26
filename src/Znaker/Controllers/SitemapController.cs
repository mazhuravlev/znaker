using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PostgreSqlProvider;
using Znaker.Models;

namespace Znaker.Controllers
{
    [Route("sitemap")]
    public class SitemapController : Controller
    {
        private readonly PostgreSqlContext _db;

        public SitemapController(PostgreSqlContext db)
        {
            _db = db;
        }

        [Route("index.xml")]
        public IActionResult Index()
        {
            // TODO: сделать настоящий индекс
            var totalMaps = Math.Ceiling(_db.Contacts.Count() / 50000D);
            var model = new List<SitemapIndexModel>();
            for (var i = 0; i < totalMaps; i++)
            {
                model.Add(new SitemapIndexModel
                {
                    Loc = $"sitemap_{i + 1}.xml",
                    Lastmod = _db.Contacts.Skip(50000 * i).Take(50000).Max(c => c.UpdatedOn).ToString("yyyy-MM-ddTHH:mm:sszzz")
                });
            }
            Response.ContentType = "application/xml";
            return View(model);
        }

        [Route("sitemap_{id}.xml")]
        public IActionResult Sitemap(int id)
        {
            Response.ContentType = "application/xml";
            return View();
        }
    }
}