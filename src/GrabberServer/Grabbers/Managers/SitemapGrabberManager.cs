using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure;

namespace GrabberServer.Grabbers.Managers
{
    public class SitemapGrabberManager
    {
        private readonly ISitemapService _sitemapService;
        private readonly AdJobsService _adJobsService;
        private readonly Dictionary<string, GrabberEntry> _grabberMap = new Dictionary<string, GrabberEntry>();
        public TimeSpan CycleDelay = TimeSpan.FromSeconds(1);

        public SitemapGrabberManager(ISitemapService sitemapService, AdJobsService adJobsService)
        {
            _sitemapService = sitemapService;
            _adJobsService = adJobsService;
        }

        public void AddGrabber(string name, ISitemapGrabber sitemapGrabber, TimeSpan? indexDownloadInterval = null,
            bool isEnabled = false)
        {
            _grabberMap.Add(
                name,
                new GrabberEntry
                {
                    Grabber = sitemapGrabber,
                    Name = name,
                    IndexDownloadInterval = indexDownloadInterval ?? TimeSpan.FromMinutes(60),
                    IsEnabled = isEnabled
                }
            );
        }

        public Task Run(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var indexGrabberEntry = GetNextIndexGrabber();
                    if (indexGrabberEntry != null)
                    {
                        _sitemapService.SaveSitemaps(indexGrabberEntry.Grabber.GrabIndex().Result);
                    }
                    var downloadGrabberEntry = GetNextDownloadGrabber();
                    if (null != downloadGrabberEntry)
                    {
                        HandleSitemapGrabResult(
                            downloadGrabberEntry.Grabber.GetSourceType(),
                            downloadGrabberEntry.Grabber.GrabNextSitemap(
                                _sitemapService.GetSitemapsForType(downloadGrabberEntry.Grabber.GetSourceType()
                                )
                            ).Result
                        );
                    }
                    try
                    {
                        Task.Delay(CycleDelay, cancellationToken).Wait(cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                }
            }, cancellationToken);
        }

        private void HandleSitemapGrabResult(SourceType sourceType, SitemapGrabResult result)
        {
            if (result.AdIds.Count > 0)
            {
                _sitemapService.MarkDownloaded(result.SitemapEntry);
                _adJobsService.StoreAdJobs(sourceType, result.AdIds);
            }
            else
            {
                throw new Exception("Got no ad ids from " + sourceType);
            }
        }

        protected class GrabberEntry
        {
            public ISitemapGrabber Grabber;
            public string Name;
            public TimeSpan IndexDownloadInterval;
            public int DownloadInteval = 1;
            public bool IsEnabled;
            public DateTime LastDownloadTime = DateTime.Now;
            public DateTime LastIndexDownloadTime = DateTime.Now.AddYears(-1);
        }

        private GrabberEntry GetNextIndexGrabber()
        {
            return _grabberMap.Values.FirstOrDefault(g =>
                g.IsEnabled &&
                g.LastIndexDownloadTime + g.IndexDownloadInterval < DateTime.Now
            );
        }

        private GrabberEntry GetNextDownloadGrabber()
        {
            return _grabberMap.Values
                .FirstOrDefault(
                    g =>
                        g.IsEnabled &&
                        g.Grabber.HasSitemapsToGrab(_sitemapService.GetSitemapsForType(g.Grabber.GetSourceType())
                )
            );
        }
    }
}