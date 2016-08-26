using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PostgreSqlProvider;
using Znaker.Models;

namespace Znaker.Controllers
{
    public class SitemapController : Controller
    {
        private readonly PostgreSqlContext _db;

        public SitemapController(PostgreSqlContext db)
        {
            _db = db;
        }

        [Route("/sitemap/index")]
        public IActionResult Index()
        {
            // TODO: сделать настоящий индекс
            var sitemapIndex = new SitemapIndex();
            var maxUpdatedAt = _db.Contacts.Take(50000).Max(c => c.UpdatedOn);
            var sitemap = new SitemapIndex.Sitemap
            {
                Loc = "sitemap_1.xml",
                Lastmod = maxUpdatedAt.ToString("yyyy-MM-ddTHH:mm:sszzz")
            };
            sitemapIndex.Sitemaps = new List<SitemapIndex.Sitemap> {sitemap};
            Response.ContentType = "application/xml";

            return View(sitemapIndex);
        }
    }
}