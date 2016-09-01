using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using OlxLib.Entities;
using OlxLib.Utils;

namespace OlxLib.Workers
{
    public class SitemapWorker
    {
        private class Sitemap
        {
            public int Number;
            public string Loc;
            public string Lastmod;
        }

        private readonly ParserContext _db;

        public SitemapWorker(ParserContext parserContext)
        {
            _db = parserContext;
        }

        public string Run(OlxConfig config)
        {
            var sitemaps = ParseIndexSitemap(
                XDocument.Parse(
                    HttpUtils.DownloadString(config.GetSitemapUrl()).Result
                )
            );
            foreach (var sitemap in sitemaps)
            {
                HandleSitemap(sitemap, config.OlxType);
            }
            //_db.SaveChanges();
            return "Ok!";
        }

        private static IEnumerable<Sitemap> ParseIndexSitemap(XDocument indexSitemapXdoc)
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var adsSitemapRegex = new Regex(@"sitemap-ads-(\d+)\.xml");
            return indexSitemapXdoc.Root.Elements(ns + "sitemap")
                .Where(s => adsSitemapRegex.Match(s.Element(ns + "loc").Value).Length > 0)
                .Select(s => new Sitemap
                {
                    Number = int.Parse(adsSitemapRegex.Match(s.Element(ns + "loc").Value).Groups[1].Value),
                    Loc = s.Element(ns + "loc").Value,
                    Lastmod = s.Element(ns + "lastmod").Value
                })
                .ToList();
        }

        private void HandleSitemap(Sitemap sitemap, OlxType olxType)
        {
            var lastmodMeta = _db.ParserMeta.FirstOrDefault(
                pm => pm.OlxType == olxType && pm.Key == GetLastmodMetaKey(sitemap)
            );
            if (null != lastmodMeta && (lastmodMeta.Value != sitemap.Lastmod))
            {
                Console.WriteLine($"Updated sitemap: {sitemap.Number}, download and process");
                if (ProcessSitemap(sitemap))
                {
                    UpdateMeta(lastmodMeta, sitemap.Lastmod);
                }
            }
            else
            {
                Console.WriteLine($"New sitemap: {sitemap.Number}, download and process");
                if (ProcessSitemap(sitemap))
                {
                    CreateLastmodMeta(sitemap, olxType);
                }
            }
        }

        private void CreateLastmodMeta(Sitemap sitemap, OlxType olxType)
        {
            var newLastmodMeta = new ParserMeta
            {
                Key = GetLastmodMetaKey(sitemap),
                OlxType = olxType,
                Value = sitemap.Lastmod
            };
            _db.ParserMeta.Add(newLastmodMeta);
        }

        private static void UpdateMeta(ParserMeta lastmodMeta, string sitemapLastmod)
        {
            lastmodMeta.Value = sitemapLastmod;
        }

        private static bool ProcessSitemap(Sitemap sitemap)
        {
            throw new NotImplementedException();
        }

        private static string GetLastmodMetaKey(Sitemap sitemap)
        {
            return $"sitemap:{sitemap.Loc}:lastmod";
        }
    }
}