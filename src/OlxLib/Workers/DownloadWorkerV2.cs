using System;
using System.Collections.Concurrent;
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
    public class DownloadWorkerV2 : BaseWorker
    {
        private readonly ConcurrentQueue<DownloadJob> _jobList = new ConcurrentQueue<DownloadJob>();
        private readonly ConcurrentQueue<DownloadJob> _processingList = new ConcurrentQueue<DownloadJob>();
        private readonly ConcurrentQueue<DownloadJob> _submittingList = new ConcurrentQueue<DownloadJob>();
        private Task [] _tasks;

        private const int QuenueSize = 30;
        public DownloadWorkerV2(IServiceProvider serviceProvider) : base(serviceProvider) { }



        //longruning task
        [Queue("Download manager")]
        public void Run(OlxType [] olxTypes, IJobCancellationToken cancellationToken)
        {
            var ct = new CancellationTokenSource();
            Run(olxTypes, ct.Token);

            while (true)
            {
                if (cancellationToken != null && cancellationToken.ShutdownToken.IsCancellationRequested)
                {
                    ct.Cancel();
                    if (_tasks != null)
                    {
                        Task.WaitAll(_tasks);
                        return;
                    }
                }
                Task.Delay(100, ct.Token).Wait(ct.Token);
            }
        }
        //for test run
        public void Run(OlxType[] olxTypes, CancellationToken cancellationToken)
        {
            if (olxTypes == null || !olxTypes.Any())
            {
                throw new ArgumentException("Olxtypes not provided");
            }

            _tasks = new[]
            {
                JobManager(olxTypes, cancellationToken),
                ProcessingManager(olxTypes, cancellationToken),
                JobSubmitter(cancellationToken)
            };

            Task.WaitAll(_tasks, cancellationToken);
        }

        private Task JobManager(OlxType[] olxTypes, CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return Task.CompletedTask;
                }

                using (var db = GetParserContext())
                {
                    foreach (var type in olxTypes)
                    {
                        var ids = _jobList.ToArray().Where(c => c.OlxType == type).Select(c => c.Id).ToList();
                        if (ids.Count >= QuenueSize) continue;

                        var list = db.DownloadJobs
                                .AsNoTracking()
                                .Where(c => ids.Contains(c.Id) == false && c.OlxType == type && c.ProcessedAt.HasValue == false)
                                .OrderBy(c => c.CreatedAt)
                                .Take(ids.Count - QuenueSize);

                        foreach (var job in list)
                        {
                            _jobList.Enqueue(job);
                        }
                    }
                }

                Task.Delay(TimeSpan.FromSeconds(20), cancellationToken).Wait(cancellationToken);
            }
        }

        protected Task ProcessingManager(OlxType[] olxTypes, CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return Task.CompletedTask;
                }

                foreach (var type in olxTypes)
                {
                    /// if _processingList has no this job type - Dequeue from _jobList, Enqueue to _processingList
                    /// run backgroung job
                    //BackgroundJob.Enqueue<DownloadWorkerV2>(c => c.DownloadJob())
                }


                Task.Delay(TimeSpan.FromMilliseconds(200), cancellationToken).Wait(cancellationToken);
            }
        }

        protected Task JobSubmitter(CancellationToken cancellationToken)
        {
            while (true)
            {
                DownloadJob job;
                while (_submittingList.TryDequeue(out job))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return Task.CompletedTask;
                    }
                    using (var db = GetParserContext())
                    {
                        db.DownloadJobs.Attach(job);
                        db.SaveChanges();
                    }
                }
                Task.Delay(TimeSpan.FromMilliseconds(200), cancellationToken).Wait(cancellationToken);
            }
        }



        [Queue("Download worker")]
        protected string JobDownloader(int jobId)
        {
            //download, Dequeue from _processingList, Enqueue to _submittingList


            return "";
        }




    }
}