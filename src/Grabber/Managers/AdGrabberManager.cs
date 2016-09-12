using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Grabber.Grabbers;
using Grabber.Managers;
using Grabber.Models;
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

        public AdGrabberManager(IAdJobsService adJobsService, ISitemapGrabberManager sitemapGrabberManager,
            ILogger<AdGrabberManager> logger)
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
                foreach (var grabberEntry in _grabberEntries)
                {
                    var grabber = grabberEntry.Value;
                    if (grabber.Jobs.Count < grabber.JobsLimit)
                    {
                        var job = _adJobsService.GetJob(grabber.Grabber.GetSourceType());
                        if (job == null)
                        {
                            _logger.LogInformation(
                                "Notified sitemap manager to get more jobs for " +
                                grabber.Grabber.GetSourceType());
                            _sitemapGrabberManager.AddJobDemand(grabber.Grabber.GetSourceType(), 10);
                            continue;
                        }
                        var o = Task.Factory.StartNew(() => grabber.Grabber.Grab(job)).ToObservable();
                        _grabberEntries[grabber.Grabber.GetSourceType().ToString()].Jobs[job.AdId] = o.Subscribe(
                            HandleResult,
                            error => HandleError(error, job)
                        );
                        _logger.LogInformation($"Added new job {job.AdId} for {grabber.Grabber.GetSourceType()}");
                    }
                    //Task.Delay(EmptyQueueDelay, cancellationToken).Wait(cancellationToken);
                }
            }
        }

        private void HandleError(Exception e, AdGrabJob job)
        {
            _logger.LogWarning(new EventId(), e, $"Grabber {job.SourceType} task {job.AdId} failed");
            RemoveTask(job.SourceType, job.AdId);
        }

        private void HandleResult(AdGrabJobResult result)
        {
            _logger.LogInformation($"Grabber task successful ({result.Contacts?.Count ?? 0} contacts): " +
                                   result.Text.Substring(0, Math.Min(result.Text.Length, 40)));
            // TODO: create export jobs
            RemoveTask(result.Job.SourceType, result.Job.AdId);
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

        public class
            GrabberEntry
        {
            public
                IAdGrabber Grabber;

            public
                bool IsEnabled = true;

            public
                int JobsLimit = 1;

            public
                Dictionary<string, IDisposable> Jobs = new Dictionary<string, IDisposable>();
        }
    }
}