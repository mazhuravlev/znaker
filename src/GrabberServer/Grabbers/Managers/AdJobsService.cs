using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrabberServer.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Razor.Parser;

namespace GrabberServer.Grabbers.Managers
{
    public interface IAdJobsService
    {
        void StoreAdJobs(SourceType sourceType, List<string> adIds);
        JobDemandResult GetJobs(JobDemand jobDemand);
    }

    public class AdJobsService : IAdJobsService
    {
        private readonly GrabberContext _grabberContext;

        public AdJobsService(GrabberContext grabberContext)
        {
            _grabberContext = grabberContext;
        }

        public void StoreAdJobs(SourceType sourceType, List<string> adIds)
        {
            var saveTasks = new List<Task>();
            foreach (var adId in adIds)
            {
                _grabberContext.AdDownloadJobs.Add(new AdDownloadJob
                {
                    SourceType = sourceType,
                    AdId = adId
                });
                saveTasks.Add(_grabberContext.SaveChangesAsync());
            }
            Task.WaitAll(saveTasks.ToArray());
        }

        public JobDemandResult GetJobs(JobDemand jobDemand)
        {
            var jobDemandResult = new JobDemandResult();
            jobDemandResult.AddRange(
                jobDemand.Select(
                    jodDemandEntry =>
                        new KeyValuePair<SourceType, List<AdDownloadJob>>(jodDemandEntry.Key,
                            GetJobsForSourceType(jodDemandEntry.Key, jodDemandEntry.Value))));
            return jobDemandResult;
        }

        public List<AdDownloadJob> GetJobsForSourceType(SourceType sourceType, int count = 1)
        {
            return _grabberContext.AdDownloadJobs.Where(adj => adj.SourceType == sourceType).Take(count).ToList();
        }
    }
}