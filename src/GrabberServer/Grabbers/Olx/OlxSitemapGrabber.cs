using System.Collections.Generic;
using System.Threading.Tasks;
using GrabberServer.Entities;

namespace GrabberServer.Grabbers.Olx
{
    public class OlxSitemapGrabber : ISitemapGrabber
    {
        private OlxConfig _olxConfig;

        public OlxSitemapGrabber(OlxConfig olxConfig)
        {
            _olxConfig = olxConfig;
        }

        public Task GrabIndex()
        {
            throw new System.NotImplementedException();
        }

        public bool HasSitemapsToGrab()
        {
            throw new System.NotImplementedException();
        }

        public Task<List<SitemapEntry>> GrabNextSitemap()
        {
            throw new System.NotImplementedException();
        }
    }
}