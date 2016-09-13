using System.Collections.Generic;
using System.Linq;
using Grabber.Entities;
using Infrastructure;
using RabbitMQ.Client;

namespace Grabber.Infrastructure.Services
{
    public class SitemapService : ISitemapService
    {
        private readonly Dictionary<SourceType, List<SitemapEntry>> _sitemaps = new Dictionary<SourceType, List<SitemapEntry>>();

        public void SaveSitemaps(SourceType sourceType, List<SitemapEntry> sitemapEntries)
        {
            if (!_sitemaps.ContainsKey(sourceType))
            {
                _sitemaps[sourceType] = sitemapEntries;
            }
            else
            {
                _sitemaps[sourceType] = sitemapEntries.Select(s =>
                {
                    var existing = _sitemaps[sourceType].FirstOrDefault(es => es.Loc == s.Loc);
                    if (existing != null)
                    {
                        s.DownloadedLastmod = s.DownloadedLastmod;
                    }
                    return s;
                }).ToList();
            }
        }

        public List<SitemapEntry> GetSitemapsForType(SourceType sourceType)
        {
            return _sitemaps.ContainsKey(sourceType) ? _sitemaps[sourceType] : new List<SitemapEntry>();
        }

        public void MarkSitemapAsDownloaded(SitemapEntry sitemapEntry)
        {
            lock (_sitemaps)
            {
                _sitemaps[sitemapEntry.SourceType] = _sitemaps[sitemapEntry.SourceType].Select(s =>
                {
                    if (s.Loc == sitemapEntry.Loc)
                    {
                        s.DownloadedLastmod = s.Lastmod;
                    }
                    return s;
                }).ToList();
            }
        }
    }
}