using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Infrastructure;
using Grabber.Entities;

namespace Grabber.Grabbers.Olx
{
    public class OlxSitemapGrabber : ISitemapGrabber
    {
        private static readonly XNamespace Ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        private static readonly Regex IdRegex = new Regex(@"ID([a-zA-Z0-9]+)");
        private static readonly Regex AdsSitemapRegex = new Regex(@"sitemap-ads-(\d+)\.xml");

        private readonly OlxConfig _config;
        private readonly Http.IGrabberHttpClient _client;

        public OlxSitemapGrabber(OlxConfig olxConfig, Http.IGrabberHttpClient httpClient)
        {
            _config = olxConfig;
            _client = httpClient;
        }

        public List<SitemapEntry> GrabIndex()
        {
                var sitemapResponse = _client.GetAsync(_config.GetSitemapUrl()).Result;
                if (!sitemapResponse.IsSuccessStatusCode)
                {
                    throw new Exception("http: sitemap request failed");
                }
                return ParseIndexSitemap(
                    XDocument.Parse(sitemapResponse.Content.ReadAsStringAsync().Result)
                );
        }

        public bool HasSitemapsToGrab(List<Entities.SitemapEntry> sitemaps)
        {
            if (0 == sitemaps.Count)
            {
                return false;
            }
            return null != GetNextSitemap(sitemaps);
        }

        public List<string> GrabNextSitemap(List<Entities.SitemapEntry> sitemaps)
        {
            var sitemap = GetNextSitemap(sitemaps);
            if (null == sitemap)
            {
                throw new Exception("no next sitemap");
            }
            var sitemapResponse = _client.GetAsync(sitemap.Loc).Result;
            if (!sitemapResponse.IsSuccessStatusCode)
            {
                throw new Exception("http sitemap request failed");
            }
            return GetIdsFromSitemap(XDocument.Parse(sitemapResponse.Content.ReadAsStringAsync().Result));
        }

        public SourceType GetSourceType()
        {
            return (SourceType) _config.OlxType;
        }

        private static SitemapEntry GetNextSitemap(IEnumerable<SitemapEntry> sitemaps)
        {
            return sitemaps.First(s => s.Lastmod != s.DownloadedLastmod);
        }

        private static List<string> GetIdsFromSitemap(XDocument sitemapXdoc)
        {
            return sitemapXdoc.Root.Elements(Ns + "url")
                .Select(u => u.Element(Ns + "loc").Value)
                .Select(s => IdRegex.Match(s).Groups[1].Value)
                .Select(id => IdUtils.DecryptOlxId(id).ToString())
                .ToList();
        }

        private List<SitemapEntry> ParseIndexSitemap(XDocument indexSitemapXdoc)
        {
            return indexSitemapXdoc.Root.Elements(Ns + "sitemap")
                .Where(s => AdsSitemapRegex.Match(s.Element(Ns + "loc").Value).Length > 0)
                .Select(s => new SitemapEntry()
                {
                    SourceType = (SourceType) _config.OlxType,
                    Loc = s.Element(Ns + "loc").Value,
                    Lastmod = s.Element(Ns + "lastmod").Value
                })
                .ToList();
        }
    }
}