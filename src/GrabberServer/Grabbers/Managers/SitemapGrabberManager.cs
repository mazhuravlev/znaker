using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure;

namespace GrabberServer.Grabbers.Managers
{
    public interface ISitemapGrabberManager
    {
        void RequestMoreJobs(JobDemand jobDemand);

        void AddGrabber(string name, ISitemapGrabber sitemapGrabber, TimeSpan? indexDownloadInterval = null,
            bool isEnabled = false);

        Task Run(CancellationToken cancellationToken);
    }

    public class SitemapGrabberManager : ISitemapGrabberManager
    {
        private readonly ISitemapService _sitemapService;
        private readonly IAdJobsService _adJobsService;
        private readonly Dictionary<string, GrabberEntry> _grabberMap = new Dictionary<string, GrabberEntry>();
        private readonly Dictionary<SourceType, int> _jobDemand = new Dictionary<SourceType, int>();
        public TimeSpan CycleDelay;

        public SitemapGrabberManager(ISitemapService sitemapService, IAdJobsService adJobsService)
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
                    Task.Delay(CycleDelay, cancellationToken).Wait(cancellationToken);
                }
            }, cancellationToken);
        }

        public void RequestMoreJobs(JobDemand jobDemand)
        {
            foreach (var jobDemandEntry in jobDemand)
            {
                if (_jobDemand.ContainsKey(jobDemandEntry.Key))
                {
                    _jobDemand[jobDemandEntry.Key] += jobDemandEntry.Value;
                }
                else
                {
                    _jobDemand[jobDemandEntry.Key] = jobDemandEntry.Value;
                }
            }
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