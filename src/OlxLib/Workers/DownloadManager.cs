using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using OlxLib.Entities;

namespace OlxLib.Workers
{
    public class DownloadManager : BaseWorker
    {
        private readonly ConcurrentQueue<JobItem> _waitingList = new ConcurrentQueue<JobItem>();

        private readonly ConcurrentDictionary<int, JobItem> _processingList = new ConcurrentDictionary<int, JobItem>();

        private readonly ConcurrentQueue<OlxDownloadResult> _submittingList = new ConcurrentQueue<OlxDownloadResult>();
        private static DownloadWorker DownloadWorker => new DownloadWorker(new HttpClient());
        private const int QuenueSize = 300;
        private const int QueueThrottleSec = 360;
        private Task[] _tasks;


        public DownloadManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }



        //longruning task
        [Queue("download_manager")]
        [AutomaticRetry(Attempts = 0)]
        public void Run(OlxType olxType, IJobCancellationToken cancellationToken)
        {
            var cts = new CancellationTokenSource();
            Run(olxType, cts.Token);

            try
            {
                while (true)
                {
                    cancellationToken?.ThrowIfCancellationRequested();
                    Task.Delay(200, cts.Token).Wait(cts.Token);
                }
            }
            catch (Exception)
            {
                cts.Cancel();
                try
                {
                    Task.WaitAll(_tasks);
                }
                catch (Exception)
                {
                    // ignored
                }
                throw;
            }
        }
        //for test run
        public void Run(OlxType olxType, CancellationToken cancellationToken)
        {
            _tasks = new Task[3];
            _tasks[0] = new Task(() => ProcessingManager(cancellationToken), cancellationToken);
            _tasks[0].Start();

            _tasks[1] = new Task(() => JobManager(olxType, cancellationToken), cancellationToken);
            _tasks[1].Start();

            _tasks[2] = new Task(() => JobSubmitter(cancellationToken), cancellationToken);
            _tasks[2].Start();
        }

        private void JobManager(OlxType olxType, CancellationToken cancellationToken)
        {
            var queueThrottleList = new List<KeyValuePair<int, DateTime>>();
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                using (var db = GetParserContext())
                {
                    if (!_waitingList.Any())
                    {
                        var list = db.DownloadJobs
                                .AsNoTracking()
                                .OrderBy(c => c.CreatedAt)
                                .Where(c => c.OlxType == olxType && c.ProcessedAt.HasValue == false)
                                .Take(QuenueSize)
                                .Select(c => new JobItem { JobId = c.Id, AdvId = c.AdvId, OlxType = c.OlxType})
                                .ToList();

                        //clean possable duplicates
                        list.RemoveAll(c => queueThrottleList.Any(z => z.Key == c.JobId));
                        if (!list.Any())
                        {
                            //#ToDo time to redownload DownloadJobs with errors and other shit
                            //list.RemoveAll(c => queueThrottleList.Any(z => z.Key == c.Id));
                        }
                        if (!list.Any()) // no new job, sleep
                        {
                            Task.Delay(TimeSpan.FromSeconds(60), cancellationToken).Wait(cancellationToken);
                        }

                        queueThrottleList.RemoveAll(c => c.Value < DateTime.Now.AddSeconds(-QueueThrottleSec));
                        foreach (var job in list)
                        {
                            queueThrottleList.Add(new KeyValuePair<int, DateTime>(job.JobId, DateTime.Now));
                            _waitingList.Enqueue(job);
                        }
                    }
                }
                Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken).Wait(cancellationToken);
            }
        }

        private void ProcessingManager(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var processingJobsCount = _processingList.Count;
                var degreeOfParallelism = 1;//TODO: add degree of parallelism to olxConfig
                var jobsToAdd = Math.Max(0, degreeOfParallelism - processingJobsCount);
                for (var i = 0; i < jobsToAdd; i++)
                {
                    JobItem job;
                    if (_waitingList.TryDequeue(out job))
                    {
                        if (_processingList.TryAdd(job.JobId, job))
                        {
                            BackgroundJob.Enqueue<DownloadManager>(c => c.JobDownloader(job.JobId));
                        }
                    }
                }
                Task.Delay(TimeSpan.FromMilliseconds(200), cancellationToken).Wait(cancellationToken);
            }
        }

        [Queue("download_worker")]
        [AutomaticRetry(Attempts = 0)]
        public string JobDownloader(int jobId)
        {
            JobItem job;
            if (!_processingList.TryRemove(jobId, out job))
            {
                throw new Exception("job not found");
            }

            var res = DownloadWorker.Run(job.AdvId, job.OlxType);
            res.JobId = jobId;
            _submittingList.Enqueue(res);
            if (res.AdHttpStatusCode.HasValue)
            {
                
            }
            return $"ad:{res.AdHttpStatusCode};c:{res.ContactsHttpStatusCode}";
        }

        protected void JobSubmitter(CancellationToken cancellationToken)
        {
            while (true)
            {
                OlxDownloadResult result;
                while (_submittingList.TryDequeue(out result))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                    using (var db = GetParserContext())
                    {
                        var job = db.DownloadJobs.FirstOrDefault(c => c.Id == result.JobId);
                        if (job == null)
                        {
                            continue; // but how?
                        }
                        job.UpdatedAt = DateTime.Now;
                        job.AdHttpStatusCode = result.AdHttpStatusCode;
                        job.ContactsHttpStatusCode = result.ContactsHttpStatusCode;
                        job.ProcessedAt = result.ProcessedAt;
                        if (result.OlxAdvert != null)
                        {
                            job.ExportJob = new ExportJob
                            {
                                CreatedAt = DateTime.Now,
                                Data = result.OlxAdvert
                            };
                        }
                        db.SaveChanges();
                    }
                }
                Task.Delay(TimeSpan.FromMilliseconds(200), cancellationToken).Wait(cancellationToken);
            }
        }

        protected class JobItem
        {
            public int JobId;
            public int AdvId;
            public OlxType OlxType;
        }
    }
}