using System;
using System.Collections;
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
        private readonly ParserContext _db;

        public SitemapWorker(ParserContext parserContext)
        {
            _db = parserContext;
        }

        public string Run(OlxConfig config)
        {
            var sitemaps = SitemapUtils.ParseIndexSitemap(
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


        private void HandleSitemap(SitemapModel sitemap, OlxType olxType)
        {
            var lastmodMeta = _db.ParserMeta.FirstOrDefault(
                pm => pm.OlxType == olxType && pm.Key == GetLastmodMetaKey(sitemap)
            );
            if (null == lastmodMeta)
            {
                ProcessSitemap(sitemap);
                CreateLastmodMeta(sitemap, olxType);
            }
            else
            {
                if (lastmodMeta.Value == sitemap.Lastmod) return;
                ProcessSitemap(sitemap);
                UpdateMeta(lastmodMeta, sitemap.Lastmod);
            }
        }

        private void ProcessSitemap(SitemapModel sitemap)
        {
            CreateDownloadJobs(SitemapUtils.GetIdsFromSitemap(
                    XDocument.Parse(
                        HttpUtils.DownloadString(sitemap.Loc).Result
                    )
                )
            );
        }

        private void CreateDownloadJobs(ICollection adIds)
        {
            throw new NotImplementedException();
        }

        private void CreateLastmodMeta(SitemapModel sitemap, OlxType olxType)
        {
            _db.ParserMeta.Add(
                new ParserMeta
                {
                    Key = GetLastmodMetaKey(sitemap),
                    OlxType = olxType,
                    Value = sitemap.Lastmod
                }
            );
            //_db.SaveChanges();
        }

        private static void UpdateMeta(ParserMeta lastmodMeta, string sitemapLastmod)
        {
            lastmodMeta.Value = sitemapLastmod;
        }

        private static string GetLastmodMetaKey(SitemapModel sitemap)
        {
            return $"sitemap:{sitemap.Loc}:lastmod";
        }

        private static class SitemapUtils
        {
            private static readonly XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

            public static List<long> GetIdsFromSitemap(XDocument sitemapXdoc)
            {
                var regex = new Regex(@"ID([a-zA-Z0-9]+)");
                return sitemapXdoc.Root.Elements(ns + "url")
                    .Select(u => u.Element(ns + "loc").Value)
                    .Select(s => regex.Match(s).Groups[1].Value)
                    .Select(IdUtils.DecryptOlxId)
                    .ToList();
            }

            public static IEnumerable<SitemapModel> ParseIndexSitemap(XDocument indexSitemapXdoc)
            {
                var adsSitemapRegex = new Regex(@"sitemap-ads-(\d+)\.xml");
                return indexSitemapXdoc.Root.Elements(ns + "sitemap")
                    .Where(s => adsSitemapRegex.Match(s.Element(ns + "loc").Value).Length > 0)
                    .Select(s => new SitemapModel
                    {
                        Number = int.Parse(adsSitemapRegex.Match(s.Element(ns + "loc").Value).Groups[1].Value),
                        Loc = s.Element(ns + "loc").Value,
                        Lastmod = s.Element(ns + "lastmod").Value
                    })
                    .ToList();
            }
        }

        public class SitemapModel
        {
            public int Number;
            public string Loc;
            public string Lastmod;
        }
    }
}