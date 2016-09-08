using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GrabberServer.Grabbers.Managers
{
    public class SitemapGrabberManager
    {
        private readonly SitemapService _sitemapService;
        private readonly Dictionary<string, GrabberEntry> _grabberMap = new Dictionary<string, GrabberEntry>();
        public int CycleDelay = 1000;

        public SitemapGrabberManager(SitemapService sitemapService)
        {
            _sitemapService = sitemapService;
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
                    var indexGrabber = GetNextIndexGrabber();
                    if (indexGrabber != null)
                    {
                        _sitemapService.SaveSitemaps(indexGrabber.Grabber.GrabIndex().Result);
                    }
                    var downloadGrabber = GetNextDownloadGrabber();
                    if (null != downloadGrabber)
                    {
                        HandleSitemapGrabResult(
                            downloadGrabber.Grabber.GrabNextSitemap(
                                _sitemapService.GetSitemapsForType(downloadGrabber.Grabber.GetSourceType()
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

        private void HandleSitemapGrabResult(SitemapGrabResult result)
        {
            throw new NotImplementedException();
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
                .Where(
                    g =>
                        g.IsEnabled &&
                        g.LastDownloadTime.AddMinutes(g.IndexDownloadInterval).CompareTo(DateTime.Now) < 0)
                .FirstOrDefault(
                    grabberEntry =>
                        grabberEntry.Grabber.HasSitemapsToGrab(
                            _sitemapService.GetSitemapsForType(grabberEntry.Grabber.GetSourceType())));
        }
    }
}