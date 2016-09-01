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
        private readonly ParserContext _db;
        private OlxConfig _config;

        public SitemapWorker(ParserContext parserContext)
        {
            _db = parserContext;
        }

        public string Run(OlxType olxType)
        {
            _config = BaseWorker.GetOlxConfig(olxType);
            var sitemaps = SitemapUtils.ParseIndexSitemap(
                XDocument.Parse(
                    HttpUtils.DownloadString(_config.GetSitemapUrl()).Result
                )
            );
            var adsCount = sitemaps.Sum(sitemap => HandleSitemap(sitemap, _config.OlxType));
            return $"Count: {adsCount}";
        }


        private int HandleSitemap(SitemapModel sitemap, OlxType olxType)
        {
            var count = 0;
            var lastmodMeta = _db.ParserMeta.FirstOrDefault(
                pm => pm.OlxType == olxType && pm.Key == GetLastmodMetaKey(sitemap)
            );
            if (null == lastmodMeta)
            {
                count = ProcessSitemap(sitemap);
                CreateLastmodMeta(sitemap, olxType);
            }
            else
            {
                if (lastmodMeta.Value == sitemap.Lastmod) return 0;
                count = ProcessSitemap(sitemap);
                UpdateMeta(lastmodMeta, sitemap.Lastmod);
            }
            return count;
        }

        private int ProcessSitemap(SitemapModel sitemap)
        {
            var ids = SitemapUtils.GetIdsFromSitemap(
                XDocument.Parse(
                    HttpUtils.DownloadString(sitemap.Loc).Result
                )
            ).ToList();
            CreateDownloadJobs(ids);
            return ids.Count;
        }

        private void CreateDownloadJobs(IEnumerable<int> adIds)
        {
            _db.DownloadJobs.AddRange(
                adIds.Select(i => new DownloadJob
                    {
                        AdvId = i,
                        OlxType = _config.OlxType,
                        CreatedAt = DateTime.Now
                    }
                )
            );
            _db.SaveChanges();
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
            _db.SaveChanges();
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

            public static IEnumerable<int> GetIdsFromSitemap(XDocument sitemapXdoc)
            {
                var regex = new Regex(@"ID([a-zA-Z0-9]+)");
                return sitemapXdoc.Root.Elements(ns + "url")
                    .Select(u => u.Element(ns + "loc").Value)
                    .Select(s => regex.Match(s).Groups[1].Value)
                    .Select(IdUtils.DecryptOlxId);
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