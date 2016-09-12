using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grabber.Grabbers;
using Grabber.Models;
using Infrastructure;
using Microsoft.Extensions.Logging;

namespace Grabber.Managers
{
    public interface ISitemapGrabberManager
    {
        void AddGrabber(string name, ISitemapGrabber sitemapGrabber, TimeSpan? indexDownloadInterval = null,
            bool isEnabled = false);

        Task Run(CancellationToken cancellationToken);

        void AddJobDemand(SourceType sourceType, int quantity);
    }

    public class SitemapGrabberManager : ISitemapGrabberManager
    {
        private readonly ISitemapService _sitemapService;
        private readonly IAdJobsService _adJobsService;
        private readonly ILogger _logger;

        private readonly Dictionary<string, GrabberEntry> _grabberMap = new Dictionary<string, GrabberEntry>();
        private readonly Dictionary<SourceType, int> _jobDemand = new Dictionary<SourceType, int>();
        public TimeSpan CycleDelay;

        public SitemapGrabberManager(ISitemapService sitemapService, IAdJobsService adJobsService, ILogger<SitemapGrabberManager> logger)
        {
            _sitemapService = sitemapService;
            _adJobsService = adJobsService;
            _logger = logger;
        }

        public void AddJobDemand(SourceType sourceType, int quantity)
        {
            if (_jobDemand.ContainsKey(sourceType))
            {
                _jobDemand[sourceType] += quantity;
            }
            else
            {
                _jobDemand[sourceType] = quantity;
            }
            if (_jobDemand[sourceType] < 0)
            {
                _jobDemand[sourceType] = 0;
            }
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
                        _sitemapService.SaveSitemaps(indexGrabberEntry.Grabber.GetSourceType(),
                            indexGrabberEntry.Grabber.GrabIndex());
                    }
                    var downloadGrabberEntry = GetNextDownloadGrabber();
                    if (null != downloadGrabberEntry)
                    {
                        var sitemap = downloadGrabberEntry.Grabber.GetNextSitemap(
                            _sitemapService.GetSitemapsForType(downloadGrabberEntry.Grabber.GetSourceType()
                            )
                        );
                        HandleSitemapGrabResult(
                            downloadGrabberEntry.Grabber.GetSourceType(),
                            downloadGrabberEntry.Grabber.GrabSitemap(sitemap)
                        );
                        _sitemapService.MarkSitemapAsDownloaded(sitemap);
                    }
                    Task.Delay(CycleDelay, cancellationToken).Wait(cancellationToken);
                }
            }, cancellationToken);
        }

        private void HandleSitemapGrabResult(SourceType sourceType, List<string> adIds)
        {
            AddJobDemand(sourceType, -adIds.Count);
            _logger.LogInformation($"Got {adIds.Count} ad ids from {sourceType}, job demand is now {_jobDemand[sourceType]}");
            _jobDemand[sourceType] -= adIds.Count;
            foreach (var adId in adIds)
            {
                _adJobsService.PushAdJob(new AdGrabJob
                {
                    SourceType = sourceType,
                    AdId = adId
                });
            }
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
            var desirousSources = _jobDemand.Where(j => j.Value > 0).OrderByDescending(j => j.Value).ToList();
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