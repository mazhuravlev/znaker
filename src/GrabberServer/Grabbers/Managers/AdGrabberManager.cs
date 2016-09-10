using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using PostgreSqlProvider;
using PostgreSqlProvider.Entities;

namespace GrabberServer.Grabbers.Managers
{
    public interface IAdGrabberManager
    {
    }

    public class AdGrabberManager : IAdGrabberManager
    {
        private readonly IAdJobsService _adJobsService;
        private readonly ISitemapGrabberManager _sitemapGrabberManager;

        private readonly Dictionary<string, GrabberEntry> _grabberEntries = new Dictionary<string, GrabberEntry>();
        private Task _processFinishedJobs;

        private static readonly TimeSpan CycleDelay = TimeSpan.FromSeconds(1);

        public AdGrabberManager(IAdJobsService adJobsService,
            ISitemapGrabberManager sitemapGrabberManager = null)
        {
            _adJobsService = adJobsService;
            _sitemapGrabberManager = sitemapGrabberManager;
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
            _processFinishedJobs = Task.Run(() => ProcessFinishedJobs(), cancellationToken);
            while (true)
            {
                if (_processFinishedJobs.IsFaulted)
                {
                    throw new Exception("Result processing task is failed");
                }
                var jobDemand = GetJobDemand();
                var jobs = _adJobsService.GetJobs(jobDemand);
                if (!jobs.DoesSatisfyDemand(jobDemand))
                {
                    _sitemapGrabberManager?.RequestMoreJobs(jobDemand);
                }
                foreach (var job in jobs)
                {
                    var grabberEntry = _grabberEntries.First(ge => ge.Value.Grabber.GetSourceType() == job.Key);
                    if (grabberEntry.Value.IsEnabled)
                    {
                        grabberEntry.Value.Jobs.AddRange(
                            job.Value.Select(aj => Task.Run(() => grabberEntry.Value.Grabber.Grab(aj))));
                    }
                }
                Task.Delay(CycleDelay, cancellationToken).Wait(cancellationToken);
            }
        }

        private void ProcessFinishedJobs()
        {
            while (true)
            {
                if (!HasFinishedJobs())
                {
                    Task.Delay(CycleDelay);
                }
                else
                {
                    throw new Exception("OK: Has finished jobs!");
                    // TODO: save finished jobs results and delete their tasks
                }
            }
        }

        public class GrabberEntry
        {
            public IAdGrabber Grabber;
            public bool IsEnabled = true;
            public List<Task<AdGrabResult>> Jobs = new List<Task<AdGrabResult>>();
            public int JobsLimit = 1;
        }

        private JobDemand GetJobDemand()
        {
            var list = _grabberEntries.Values.Where(g => g.IsEnabled)
                .Select(
                    g => new KeyValuePair<SourceType, int>(g.Grabber.GetSourceType(), g.Jobs.Count - g.JobsLimit))
                .ToList();
            return JobDemand.FromList(list);
        }

        private bool HasFinishedJobs()
        {
            return _grabberEntries.Values.Select(ge => ge.Jobs.Count(j => j.IsCompleted)).Sum(r => r) > 0;
        }
    }
}