using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Server;
using Hangfire.Storage.Monitoring;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OlxLib.Entities;

namespace OlxLib.Workers
{
    public class DownloadManager : BaseWorker
    {
        private readonly ConcurrentQueue<DownloadJob> _waitingList = new ConcurrentQueue<DownloadJob>();
        private readonly ConcurrentDictionary<int, DownloadJob> _processingList = new ConcurrentDictionary<int, DownloadJob>();
        private readonly ConcurrentQueue<OlxDownloadResult> _submittingList = new ConcurrentQueue<OlxDownloadResult>();
        private static DownloadWorker DownloadWorker => new DownloadWorker(new HttpClient());
        private const int QuenueSize = 100;
        private Task[] _tasks;

        public DownloadManager(IServiceProvider serviceProvider) : base(serviceProvider) { }



        //longruning task
        [Queue("Download manager")]
        public void Run(OlxType olxType, IJobCancellationToken cancellationToken)
        {
            var cts = new CancellationTokenSource();
            Run(olxType, cts.Token);

            while (true)
            {
                if (cancellationToken != null && cancellationToken.ShutdownToken.IsCancellationRequested)
                {
                    cts.Cancel();
                    if (_tasks != null)
                    {
                        Task.WaitAll(_tasks);
                        return;
                    }
                }
                Task.Delay(100, cts.Token).Wait(cts.Token);
            }
        }
        //for test run
        public void Run(OlxType olxType, CancellationToken cancellationToken)
        {
            _tasks = new[]
            {
                JobManager(olxType, cancellationToken),
                ProcessingManager(olxType, cancellationToken),
                JobSubmitter(cancellationToken),
            };
        }

        private Task JobManager(OlxType olxType, CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return Task.CompletedTask;
                }

                using (var db = GetParserContext())
                {
                    if (!_waitingList.Any())
                    {
                        var ids = new List<int>();
                        ids.AddRange(_processingList.Keys);
                        ids.AddRange(_submittingList.Select(c => c.DownloadJob.Id));

                        var list = db.DownloadJobs
                                .AsNoTracking()
                                .OrderBy(c => c.CreatedAt)
                                .Where
                                (c =>
                                    ids.Contains(c.Id) == false &&
                                    c.OlxType == olxType &&
                                    c.ProcessedAt.HasValue == false
                                )
                                .OrderBy(c => c.CreatedAt)
                                .Take(QuenueSize);

                        if (!list.Any())
                        {
                            //#ToDo time to redownload DownloadJobs with errors and other shit
                        }
                        if (!list.Any()) // no new job, sleep
                        {
                            Task.Delay(TimeSpan.FromSeconds(60), cancellationToken).Wait(cancellationToken);
                        }

                        foreach (var job in list)
                        {
                            _waitingList.Enqueue(job);
                        }
                    }
                }
                Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken).Wait(cancellationToken);
            }
        }

        protected Task ProcessingManager(OlxType olxType, CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return Task.CompletedTask;
                }

                var processingJobsCount = _processingList.Count;
                var degreeOfParallelism = 1;//TODO: add degree of parallelism to olxConfig
                var jobsToAdd = Math.Max(0, degreeOfParallelism - processingJobsCount);
                for (var i = 0; i < jobsToAdd; i++)
                {
                    DownloadJob job;
                    if (_waitingList.TryDequeue(out job))
                    {
                        if (_processingList.TryAdd(job.Id, job))
                        {
                            BackgroundJob.Enqueue<DownloadManager>(c => c.JobDownloader(job.Id));
                        }
                    }
                }
                Task.Delay(TimeSpan.FromMilliseconds(200), cancellationToken).Wait(cancellationToken);
            }
        }

        protected Task JobSubmitter(CancellationToken cancellationToken)
        {
            while (true)
            {
                OlxDownloadResult result;
                while (_submittingList.TryDequeue(out result))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return Task.CompletedTask;
                    }
                    using (var db = GetParserContext())
                    {
                        var job = db.DownloadJobs.FirstOrDefault(c => c.Id == result.DownloadJob.Id);
                        if (job == null)
                        {
                            continue; // but how?
                        }
                        job.UpdatedAt = result.DownloadJob.UpdatedAt;
                        job.AdHttpStatusCode = result.DownloadJob.AdHttpStatusCode;
                        job.ContactsHttpStatusCode = result.DownloadJob.ContactsHttpStatusCode;
                        job.ProcessedAt = result.DownloadJob.ProcessedAt;
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

        [Queue("Download worker")]
        [AutomaticRetry(Attempts = 0)]
        protected string JobDownloader(int jobId)
        {
            DownloadJob job;
            if (!_processingList.TryRemove(jobId, out job))
            {
                throw new Exception("job not found");
            }

            var res = DownloadWorker.Run(job);
            _submittingList.Enqueue(res);
            return "ok";
        }
    }
}