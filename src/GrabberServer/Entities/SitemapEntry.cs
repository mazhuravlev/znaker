using Infrastructure;

namespace GrabberServer.Entities
{
    public class SitemapEntry
    {
        public int Id { get; set; }
        public SourceType SourceType { get; set; }
        public string Loc { get; set; }
        public string Lastmod { get; set; }
        public string DownloadedLastmod { get; set; }
    }
}