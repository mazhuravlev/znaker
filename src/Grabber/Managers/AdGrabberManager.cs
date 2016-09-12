using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grabber.Grabbers;
using Grabber.Managers;
using Infrastructure;
using Microsoft.Extensions.Logging;
using PostgreSqlProvider;
using PostgreSqlProvider.Entities;

namespace Grabber.Managers
{
    public interface IAdGrabberManager
    {
        Task Run(CancellationToken cancellationToken);

        void AddGrabber(string name, IAdGrabber grabber);
    }

    public class AdGrabberManager : IAdGrabberManager
    {
        private readonly IAdJobsService _adJobsService;
        private readonly ISitemapGrabberManager _sitemapGrabberManager;
        private readonly ILogger _logger;

        private readonly Dictionary<string, GrabberEntry> _grabberEntries = new Dictionary<string, GrabberEntry>();
        private static readonly TimeSpan EmptyQueueDelay = TimeSpan.FromSeconds(1);

        public AdGrabberManager(IAdJobsService adJobsService, ISitemapGrabberManager sitemapGrabberManager, ILogger<AdGrabberManager> logger)
        {
            _adJobsService = adJobsService;
            _sitemapGrabberManager = sitemapGrabberManager;
            _logger = logger;
        }

        public void AddGrabber(string name, IAdGrabber grabber)
        {
            _grabberEntries.Add(name, new GrabberEntry
            {
                Grabber = grabber
            });
        }

        public Task Run(CancellationToken cancellationToken)
        {
            return Task.Run(() => DoRun(cancellationToken), cancellationToken);
        }

        private void DoRun(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Task.Delay(100, cancellationToken).Wait(cancellationToken); // tipa downloads
                var job = _adJobsService.GetJob(SourceType.OlxUa);
                if (job == null)
                {
                    _logger.LogInformation("No jobs was received, so notified sitemap manager to get more jobs");
                    _sitemapGrabberManager.AddJobDemand(SourceType.OlxUa, 10);
                    Task.Delay(EmptyQueueDelay, cancellationToken).Wait(cancellationToken);
                }
            }
        }

        public class GrabberEntry
        {
            public IAdGrabber Grabber;
            public bool IsEnabled = true;
            // TODO: some type of async jobs register
            public int JobsLimit = 1;
        }
    }
}