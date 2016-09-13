using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabber.Entities;
using Infrastructure;

namespace Grabber.Infrastructure
{
    public interface ISitemapService
    {
        void SaveSitemaps(SourceType sourceType, List<SitemapEntry> sitemapEntries);

        List<SitemapEntry> GetSitemapsForType(SourceType sourceType);

        void MarkSitemapAsDownloaded(SitemapEntry sitemapEntry);
    }
}
