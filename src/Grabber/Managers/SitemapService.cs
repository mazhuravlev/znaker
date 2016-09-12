using System.Collections.Generic;
using Grabber.Entities;
using Infrastructure;

namespace Grabber.Managers
{
    public interface ISitemapService
    {
        void SaveSitemaps(SourceType sourceType, List<SitemapEntry> sitemapEntries);

        List<SitemapEntry> GetSitemapsForType(SourceType sourceType);
    }

    public class SitemapService : ISitemapService
    {
        private readonly Dictionary<SourceType, List<SitemapEntry>> _sitemaps =
            new Dictionary<SourceType, List<SitemapEntry>>();

        public void SaveSitemaps(SourceType sourceType, List<SitemapEntry> sitemapEntries)
        {
            // overwrite all sitemaps from index
            _sitemaps[sourceType] = sitemapEntries;
        }

        public List<SitemapEntry> GetSitemapsForType(SourceType sourceType)
        {
            return _sitemaps.ContainsKey(sourceType)
                ? _sitemaps[sourceType]
                : new List<SitemapEntry>();
        }
    }
}