using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Infrastructure;
using Grabber.Entities;

namespace Grabber.Infrastructure
{
    public interface ISitemapGrabber
    {
        List<SitemapEntry> GrabIndex();

        bool HasSitemapsToGrab(List<SitemapEntry> sitemaps);

        List<string> GrabSitemap(SitemapEntry sitemap);

        SitemapEntry GetNextSitemap(List<SitemapEntry> sitemaps);

        SourceType GetSourceType();
    }
}