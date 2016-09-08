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
        public int CycleDelay = 1000;

        public SitemapGrabberManager(ISitemapService sitemapService, AdJobsService adJobsService)
        {
            _sitemapService = sitemapService;
            _adJobsService = adJobsService;
        }

        public void AddGrabber(string name, ISitemapGrabber sitemapGrabber, int indexDownloadInterval = 60,
            bool isEnabled = false)
        {
            _grabberMap.Add(
                name,
                new GrabberEntry
                {
                    Grabber = sitemapGrabber,
                    Name = name,
                    IndexDownloadInterval = indexDownloadInterval,
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
                throw new Exception("Got no ad ids from " + sourceType.ToString());
            }
        }

        protected class GrabberEntry
        {
            public ISitemapGrabber Grabber;
            public string Name;
            public int IndexDownloadInterval;
            public int DownloadInteval = 1;
            public bool IsEnabled;
            public DateTime LastDownloadTime = DateTime.Now;
            public DateTime? LastIndexDownloadTime = null;
        }

        private GrabberEntry GetNextIndexGrabber()
        {
            var newGrabber = _grabberMap.Values.FirstOrDefault(g => g.IsEnabled && null == g.LastIndexDownloadTime);
            if (null != newGrabber) return newGrabber;
            return _grabberMap.Values
                .Where(g => g.IsEnabled && null != g.LastIndexDownloadTime)
                .FirstOrDefault(
                    g =>
                        ((DateTime) g.LastIndexDownloadTime).AddMinutes(g.IndexDownloadInterval)
                            .CompareTo(DateTime.Now) < 0);
        }

        private GrabberEntry GetNextDownloadGrabber()
        {
            return _grabberMap.Values
                .Where(g => g.IsEnabled)
                .FirstOrDefault(
                    grabberEntry =>
                        grabberEntry.Grabber.HasSitemapsToGrab(
                            _sitemapService.GetSitemapsForType(grabberEntry.Grabber.GetSourceType())));
        }
    }
}