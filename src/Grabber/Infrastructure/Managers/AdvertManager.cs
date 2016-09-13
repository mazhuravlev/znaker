using System;
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

        private readonly Dictionary<string, AdvertEntry> _grabberEntries = new Dictionary<string, AdvertEntry>();
        private static readonly TimeSpan QueueDelay = TimeSpan.FromMilliseconds(100);

        public AdvertManager(IAdvertService advertService, ISitemapManager sitemapManager, ILogger<AdvertManager> logger)
        {
            _advertService = advertService;
            _sitemapManager = sitemapManager;
            _logger = logger;
        }

        public void AddGrabber(string name, IAdvertGrabber grabber)
        {
            _grabberEntries.Add(name, new AdvertEntry { Grabber = grabber });
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
                    if (entry.Jobs.Count >= entry.JobsLimit)
                    {
                        continue;
                    }

                    var job = _advertService.GetJob(entry.Grabber.GetSourceType());
                    if (job == null)
                    {
                        _logger.LogInformation("Notified sitemap manager to get more jobs for " + entry.Grabber.GetSourceType());
                        _sitemapManager.AddJobDemand(entry.Grabber.GetSourceType(), 10);
                        continue;
                    }
                    var o = Task.Factory.StartNew(() => entry.Grabber.Process(job)).ToObservable();
                    _grabberEntries[entry.Grabber.GetSourceType().ToString()].Jobs[job.Id] = o.Subscribe(
                        HandleResult,
                        error => HandleError(error, job)
                    );
                    _logger.LogInformation($"Added new job {job.Id} for {entry.Grabber.GetSourceType()}");
                }
                Task.Delay(QueueDelay, cancellationToken).Wait(cancellationToken);
            }
        }

        private void HandleError(Exception e, AdvertJob job)
        {
            _logger.LogWarning(new EventId(), e, $"Grabber {job.SourceType} task {job.Id} failed");
            RemoveTask(job.SourceType, job.Id);
        }

        private void HandleResult(AdvertJobResult result)
        {
            _logger.LogInformation($"Grabber task successful ({result.Contacts?.Count ?? 0} contacts): " +
                                   result.Text?.Substring(0, Math.Min(result.Text.Length, 40)));
            // TODO: create export jobs
            RemoveTask(result.Job.SourceType, result.Job.Id);
        }

        private void RemoveTask(SourceType sourceType, string task)
        {
            lock (_grabberEntries)
            {
                var dict = _grabberEntries[sourceType.ToString()].Jobs;
                dict[task].Dispose();
                dict.Remove(task);
                _logger.LogInformation($"Job count for {sourceType} is {dict.Count}");
            }
        }
    }
}