using Infrastructure;

namespace GrabberServer.Entities
{
    public class SitemapEntry
    {
        public int Id;
        public SourceType SourceType;
        public string Loc;
        public string Lastmod;
        public string DownloadedLastmod;
    }
}