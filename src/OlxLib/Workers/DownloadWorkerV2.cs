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

        private const int QuenueSize = 30;
        public DownloadWorkerV2(IServiceProvider serviceProvider) : base(serviceProvider) { }



        //longruning task
        [Queue("Download manager")]
        public void Run(OlxType [] olxTypes, IJobCancellationToken cancellationToken)
        {
            if (olxTypes == null || !olxTypes.Any())
            {
                throw new ArgumentException("Olxtypes not provided");
            }
            var jobManagerCanCancellationToken = new CancellationTokenSource();
            Task.Run(() => QueueManager(olxTypes, jobManagerCanCancellationToken.Token), jobManagerCanCancellationToken.Token);
            Task.Run(() => JobSubmitter(jobManagerCanCancellationToken.Token), jobManagerCanCancellationToken.Token);

            while (true)
            {
                if (cancellationToken != null && cancellationToken.ShutdownToken.IsCancellationRequested)
                {
                    jobManagerCanCancellationToken.Cancel();
                    break;
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

                for (var i = 0; i < 20; i++)
                {
                    if (cancellationToken != null && cancellationToken.ShutdownToken.IsCancellationRequested)
                    {
                        break;
                    }
                    Task.Delay(TimeSpan.FromSeconds(1), jobManagerCanCancellationToken.Token).Wait(jobManagerCanCancellationToken.Token);
                }
            }
        }

        protected void QueueManager(OlxType[] olxTypes, CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
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

        protected void JobSubmitter(CancellationToken cancellationToken)
        {
            while (true)
            {
                DownloadJob job;
                while (_submittingList.TryDequeue(out job))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
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
        protected string DownloadJob(int jobId)
        {
            //download, Dequeue from _processingList, Enqueue to _submittingList


            return "";
        }




    }
}