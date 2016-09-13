using System;

namespace Grabber.Infrastructure.Entries
{
    public class SitemapEntry
    {
        public ISitemapGrabber Grabber;
        public string Name;
        public TimeSpan IndexDownloadInterval;
        public bool IsEnabled = true;
        public DateTime LastDownloadTime = DateTime.Now.AddYears(-1);
        public DateTime LastIndexDownloadTime = DateTime.Now.AddYears(-1);
    }
}
