using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Grabber.Infrastructure.Entries;
using Grabber.Models;
using Infrastructure;
using Microsoft.Extensions.Logging;

namespace Grabber.Infrastructure.Managers
{
    public class AdvertManager : IAdvertManager
    {
        private readonly IAdvertService _advertService;
        private readonly ISitemapManager _sitemapManager;
        private readonly ILogger _logger;

        private readonly ConcurrentDictionary<string, AdvertEntry> _grabberEntries =
            new ConcurrentDictionary<string, AdvertEntry>();

        private static readonly TimeSpan QueueDelay = TimeSpan.FromMilliseconds(100);

        public AdvertManager(IAdvertService advertService, ISitemapManager sitemapManager, ILogger<AdvertManager> logger)
        {
            _advertService = advertService;
            _sitemapManager = sitemapManager;
            _logger = logger;
        }

        public void AddGrabber(string name, IAdvertGrabber grabber)
        {
            _grabberEntries[name] = new AdvertEntry {Grabber = grabber};
        }

        public Task Run(CancellationToken cancellationToken)
        {
            return Task.Run(() => DoRun(cancellationToken), cancellationToken);
        }

        private void DoRun(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                foreach (var grabberEntry in _grabberEntries)
                {
                    var entry = grabberEntry.Value;
                    if (entry.RunningJobsCount < 0)
                    {
                        throw new Exception($"RunningJobsCount < 0 for {entry.Grabber.GetSourceType()}");
                    }
                    if (entry.RunningJobsCount == entry.JobsLimit) continue;
                    var job = _advertService.GetJob(entry.Grabber.GetSourceType());
                    if (job == null)
                    {
                        _logger.LogInformation("Notified sitemap manager to get more jobs for " +
                                               entry.Grabber.GetSourceType());
                        _sitemapManager.AddJobDemand(entry.Grabber.GetSourceType(), 10);
                        continue;
                    }
                    Task.Factory.StartNew(() => entry.Grabber.Process(job), cancellationToken).ToObservable().Subscribe(
                        result => HandleResult(result, entry),
                        error => HandleError(error, entry, job)
                    );
                    entry.RunningJobsCount++;
                    _logger.LogTrace($"Added new job {job.Id} for {entry.Grabber.GetSourceType()}");
                }
                Task.Delay(QueueDelay, cancellationToken).Wait(cancellationToken);
            }
        }

        private void HandleError(Exception e, AdvertEntry entry, AdvertJob job)
        {
            _logger.LogWarning(new EventId(), e, $"Grabber {job.SourceType} task {job.Id} failed");
            entry.RunningJobsCount--;
        }

        private void HandleResult(AdvertJobResult result, AdvertEntry entry)
        {
            _logger.LogInformation($"({entry.RunningJobsCount} jobs) Grabber task successful ({result.Contacts?.Count ?? 0} contacts): " +
                                   result.Text?.Substring(0, Math.Min(result.Text.Length, 40)));
            // TODO: create export jobs
            entry.RunningJobsCount--;
        }
    }
}