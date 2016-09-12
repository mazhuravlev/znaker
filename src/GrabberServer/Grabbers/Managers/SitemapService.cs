using System;
using System.Collections.Generic;
using System.Linq;
using GrabberServer.Entities;
using Infrastructure;

namespace GrabberServer.Grabbers.Managers
{
    public interface ISitemapService
    {
        void SaveSitemaps(List<SitemapEntry> sitemapEntries);

        List<SitemapEntry> GetSitemapsForType(SourceType sourceType);

        void MarkDownloaded(SitemapEntry sitemapEntry);
    }

    public class SitemapService : ISitemapService
    {
        private readonly GrabberContext _grabberContext;

        public SitemapService(GrabberContext grabberContext)
        {
            _grabberContext = grabberContext;
        }

        public List<SitemapEntry> GetSitemapsForType(SourceType sourceType)
        {
            return _grabberContext.SitemapEntries.Where(s => s.SourceType == sourceType).ToList();
        }

        public void SaveSitemaps(List<SitemapEntry> sitemapEntries)
        {
            foreach (var sitemapEntry in sitemapEntries)
            {
                var existingSitemapEntry =
                    _grabberContext.SitemapEntries.FirstOrDefault(
                        se => se.Loc == sitemapEntry.Loc && se.SourceType == sitemapEntry.SourceType);
                if (existingSitemapEntry == null)
                {
                    _grabberContext.Add(sitemapEntry);
                }
                else
                {
                    existingSitemapEntry.Lastmod = sitemapEntry.Lastmod;
                }
                _grabberContext.SaveChanges();
            }
        }

        public void MarkDownloaded(SitemapEntry sitemapEntry)
        {
            sitemapEntry.DownloadedLastmod = sitemapEntry.Lastmod;
            _grabberContext.SaveChanges();
        }
    }
}