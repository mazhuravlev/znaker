using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using OlxLib.Entities;

namespace OlxLib.Workers
{
    public class SitemapWorker : BaseWorker
    {
        private class Sitemap
        {
            public int Number;
            public string Loc;
            public DateTime Lastmod;
        }

        public SitemapWorker(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public string Run(OlxType type)
        {
            var config = GetOlxConfig(type);
            var sitemapString = DownloadFileString(config.GetSitemapUrl()).Result;
            var xdoc = XDocument.Parse(sitemapString);
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var adsSitemapRegex = new Regex(@"sitemap-ads-(\d+)\.xml");
            var sitemaps = xdoc.Root.Elements(ns + "sitemap")
                .Where(s => adsSitemapRegex.Match(s.Element(ns + "loc").Value).Length > 0)
                .Select(s => new Sitemap
                {
                    Number = int.Parse(adsSitemapRegex.Match(s.Element(ns + "loc").Value).Groups[1].Value),
                    Loc = s.Element(ns + "loc").Value,
                    Lastmod = DateTime.Parse(s.Element(ns + "lastmod").Value)
                })
                .ToList();
            var db = GetParserContext();
            foreach (var sitemap in sitemaps)
            {
                var lastmodMetaKey = "lastmod_sitemap_" + sitemap.Number;
                var lastmodMeta = db.ParserMeta.FirstOrDefault(pm => pm.OlxType == type && pm.Key == lastmodMetaKey);
                if (null != lastmodMeta)
                {
                    // TODO: compare date, if downloaded one is newer, update and process sitemap. Skip if date is the same.
                }
                else
                {
                    // TODO: download and process sitemap
                    var newLastmodMeta = new ParserMeta
                    {
                        Key = lastmodMetaKey,
                        OlxType = type,
                        Value = sitemap.Lastmod.ToString(CultureInfo.CurrentCulture)
                    };
                    db.ParserMeta.Add(newLastmodMeta);
                    db.SaveChanges();
                }
            }

            return "Ok!";
        }

        private async Task<string> DownloadFileString(string url)
        {
            var client = new HttpClient();
            return await client.GetStringAsync(url);
        }
    }
}