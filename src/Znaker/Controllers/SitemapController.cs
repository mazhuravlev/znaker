using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using PostgreSqlProvider;
using Znaker.Models;

namespace Znaker.Controllers
{
    [Route("sitemap")]
    public class SitemapController : Controller
    {
        private const int ContactsBySitemap = 50000;
        private const string SitemapDateFormat = "yyyy-MM-ddTHH:mm:sszzz";

        private readonly ZnakerContext _db;

        public SitemapController(ZnakerContext db)
        {
            _db = db;
        }

        [Route("index.xml")]
        public IActionResult Index()
        {
            var totalMaps = Math.Ceiling(_db.Contacts.Count() / (decimal) ContactsBySitemap);
            var model = new List<SitemapIndexModel>();
            for (var i = 0; i < totalMaps; i++)
            {
                model.Add(new SitemapIndexModel
                {
                    Loc = $"https://znaker.ru/sitemap_{i + 1}.xml",
                    Lastmod =
                        _db.Contacts.Skip(ContactsBySitemap * i)
                            .Take(ContactsBySitemap)
                            .Max(c => c.UpdatedOn)
                            .ToString(SitemapDateFormat)
                });
            }
            Response.ContentType = "application/xml";
            return View(model);
        }

        [Route("sitemap_{id}.xml")]
        public IActionResult Sitemap(int id)
        {
            var urls = _db.Contacts.Skip(ContactsBySitemap * (id - 1))
                .Take(ContactsBySitemap)
                .Select(c => new SitemapModel
                {
                    Loc = "https://znaker.ru/" + HtmlEncoder.Default.Encode(c.Identity),
                    Lastmod = c.UpdatedOn.ToString(SitemapDateFormat)
                })
                .ToList();
            Response.ContentType = "application/xml";
            return View(urls);
        }
    }
}