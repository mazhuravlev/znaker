using System;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure;

namespace Grabber.Infrastructure
{
    public interface ISitemapManager
    {
        Task Run(CancellationToken cancellationToken);
        void AddGrabber(string name, ISitemapGrabber sitemapGrabber, TimeSpan? indexDownloadInterval = null, bool isEnabled = false);
        void AddJobDemand(SourceType sourceType, int quantity);
    }
}
