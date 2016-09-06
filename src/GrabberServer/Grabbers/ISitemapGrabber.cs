using System.Collections.Generic;
using System.Threading.Tasks;
using GrabberServer.Entities;

namespace GrabberServer.Grabbers
{
    public interface ISitemapGrabber
    {
        Task GrabIndex();

        bool HasSitemapsToGrab();

        Task<List<SitemapEntry>> GrabNextSitemap();
    }
}