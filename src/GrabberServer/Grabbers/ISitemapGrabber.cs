using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using GrabberServer.Entities;

namespace GrabberServer.Grabbers
{
    public interface ISitemapGrabber
    {
        Task<List<SitemapEntry>> GrabIndex();

        bool HasSitemapsToGrab(List<SitemapEntry> sitemaps);

        Task<SitemapGrabResult> GrabNextSitemap(List<SitemapEntry> sitemaps);
    }
}