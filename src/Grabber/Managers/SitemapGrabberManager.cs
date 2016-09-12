using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grabber.Grabbers;
using Infrastructure;

namespace Grabber.Managers
{
    public interface ISitemapGrabberManager
    { 
        void AddGrabber(string name, ISitemapGrabber sitemapGrabber, TimeSpan? indexDownloadInterval = null,
            bool isEnabled = false);

        Task Run(CancellationToken cancellationToken);
    }

    public class SitemapGrabberManager : ISitemapGrabberManager
    {
        private readonly ISitemapService _sitemapService;
        private readonly Dictionary<string, GrabberEntry> _grabberMap = new Dictionary<string, GrabberEntry>();
        private readonly Dictionary<SourceType, int> _jobDemand = new Dictionary<SourceType, int>();
        public TimeSpan CycleDelay;

        public SitemapGrabberManager(ISitemapService sitemapService)
        {
            _sitemapService = sitemapService;
        }

        public void AddGrabber(string name, ISitemapGrabber sitemapGrabber, TimeSpan? indexDownloadInterval = null,
            bool isEnabled = true)
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
                        _sitemapService.SaveSitemaps(indexGrabberEntry.Grabber.GetSourceType(), indexGrabberEntry.Grabber.GrabIndex());
                    }
                    var downloadGrabberEntry = GetNextDownloadGrabber();
                    if (null != downloadGrabberEntry)
                    {
                        HandleSitemapGrabResult(
                            downloadGrabberEntry.Grabber.GetSourceType(),
                            downloadGrabberEntry.Grabber.GrabNextSitemap(
                                _sitemapService.GetSitemapsForType(downloadGrabberEntry.Grabber.GetSourceType()
                                )
                            )
                        );
                    }
                    Task.Delay(CycleDelay, cancellationToken).Wait(cancellationToken);
                }
            }, cancellationToken);
        }

        private void HandleSitemapGrabResult(SourceType sourceType, List<string> adIds)
        {
            // TODO: push ids to message queue
            throw new NotImplementedException();
        }

        protected class GrabberEntry
        {
            public ISitemapGrabber Grabber;
            public string Name;
            public TimeSpan IndexDownloadInterval;
            public bool IsEnabled = true;
            public DateTime LastDownloadTime = DateTime.Now.AddYears(-1);
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
            if (_jobDemand.Count == 0) return null;
            var desirousSources = _jobDemand.OrderByDescending(j => j.Value).ToList();
            return desirousSources
                .Select(
                    desirousSource =>
                        _grabberMap.Values.FirstOrDefault(
                            g =>
                                g.IsEnabled &&
                                g.Grabber.HasSitemapsToGrab(_sitemapService.GetSitemapsForType(g.Grabber.GetSourceType()))))
                .FirstOrDefault(grabber => grabber != null);
        }
    }
}