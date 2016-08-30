using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace OlxLib.Utils
{
    public class SitemapUtils
    {
        private const string SitemapUrlTemplate = "http://olx.ua/sitemap-ads-{0}.xml";  
        public static List<long> GetIdsFromSitemap(XDocument xdoc)
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var regex = new Regex(@"ID([a-zA-Z0-9]+)");
            return xdoc.Root.Elements(ns + "url")
                .Select(u => u.Element(ns + "loc").Value)
                .Select(s => regex.Match(s).Groups[1].Value)
                .Select(IdUtils.DecryptOlxId)
                .ToList();
        }
    }
}
