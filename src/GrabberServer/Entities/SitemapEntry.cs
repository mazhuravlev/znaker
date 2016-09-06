using Infrastructure;

namespace GrabberServer.Entities
{
    public class SitemapEntry
    {
        public SourceType SourceType;
        public string Loc;
        public string Lastmod;
        public string DownloadedLastmod;

        public void MarkDownloaded()
        {
            DownloadedLastmod = Lastmod;
        }
    }
}