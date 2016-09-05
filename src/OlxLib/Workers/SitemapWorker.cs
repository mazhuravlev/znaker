﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Hangfire;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using OlxLib.Entities;
using OlxLib.Utils;

namespace OlxLib.Workers
{
    public class SitemapWorker
    {
        private readonly ParserContext _db;
        private OlxConfig _config;
        private HttpClient _client;

        public SitemapWorker(ParserContext parserContext)
        {
            _db = parserContext;
        }
        [Queue("sitemap_download")]
        public string Run(OlxType olxType)
        {
            _config = BaseWorker.GetOlxConfig(olxType);
            _client = new HttpClient();
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36");
            var sitemapResponse = _client.GetAsync(_config.GetSitemapUrl()).Result;
            if (!sitemapResponse.IsSuccessStatusCode)
            {
                throw new Exception("Sitemap request failed");
            }
            var sitemaps = SitemapUtils.ParseIndexSitemap(
                XDocument.Parse(sitemapResponse.Content.ReadAsStringAsync().Result)
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
            var sitemapResponse = _client.GetAsync(sitemap.Loc).Result;
            if (!sitemapResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Sitemap request failed: " + sitemap.Loc);
            }
            var ids = SitemapUtils.GetIdsFromSitemap(
                XDocument.Parse(sitemapResponse.Content.ReadAsStringAsync().Result)
            ).ToList();
            CreateDownloadJobs(ids);
            return ids.Count;
        }

        private void CreateDownloadJobs(IReadOnlyCollection<int> adIds)
        {
            var existingIds = _db.DownloadJobs
                .Where(dj => _config.OlxType == dj.OlxType && dj.AdvId <= adIds.Max() && dj.AdvId >= adIds.Min())
                .Select(dj => dj.AdvId)
                .ToList();
            const int batchSize = 1000;
            for (var batchNumber = 0; batchNumber * batchSize < adIds.Count; batchNumber++)
            {
                _db.DownloadJobs.AddRange(
                    adIds.Skip(batchSize * batchNumber).Take(batchSize)
                        .Where(id => !existingIds.Contains(id))
                        .Select(id => new DownloadJob
                        {
                            AdvId = id,
                            OlxType = _config.OlxType,
                            CreatedAt = DateTime.Now
                        }
                    )
                );
                _db.SaveChanges();
            }
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
            private static readonly XNamespace Ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

            public static IEnumerable<int> GetIdsFromSitemap(XDocument sitemapXdoc)
            {
                var regex = new Regex(@"ID([a-zA-Z0-9]+)");
                return sitemapXdoc.Root.Elements(Ns + "url")
                    .Select(u => u.Element(Ns + "loc").Value)
                    .Select(s => regex.Match(s).Groups[1].Value)
                    .Select(IdUtils.DecryptOlxId);
            }

            public static IEnumerable<SitemapModel> ParseIndexSitemap(XDocument indexSitemapXdoc)
            {
                var adsSitemapRegex = new Regex(@"sitemap-ads-(\d+)\.xml");
                return indexSitemapXdoc.Root.Elements(Ns + "sitemap")
                    .Where(s => adsSitemapRegex.Match(s.Element(Ns + "loc").Value).Length > 0)
                    .Select(s => new SitemapModel
                    {
                        Number = int.Parse(adsSitemapRegex.Match(s.Element(Ns + "loc").Value).Groups[1].Value),
                        Loc = s.Element(Ns + "loc").Value,
                        Lastmod = s.Element(Ns + "lastmod").Value
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