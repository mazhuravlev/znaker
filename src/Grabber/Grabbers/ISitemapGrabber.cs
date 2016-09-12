using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Infrastructure;
using Grabber.Entities;

namespace Grabber.Grabbers
{
    public interface ISitemapGrabber
    {
        List<SitemapEntry> GrabIndex();

        bool HasSitemapsToGrab(List<SitemapEntry> sitemaps);

        List<string> GrabNextSitemap(List<SitemapEntry> sitemaps);

        SourceType GetSourceType();
    }
}