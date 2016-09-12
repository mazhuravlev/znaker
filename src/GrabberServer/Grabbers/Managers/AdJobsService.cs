using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrabberServer.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Razor.Parser;
using PostgreSqlProvider;
using PostgreSqlProvider.Entities;

namespace GrabberServer.Grabbers.Managers
{
    public interface IAdJobsService
    {
        void StoreAdJobs(SourceType sourceType, List<string> adIds);
        JobDemandResult GetJobs(JobDemand jobDemand);
        void SaveJobResult(AdJobResult jobResult);
    }

    public class AdJobsService : IAdJobsService
    {
        private readonly GrabberContext _grabberContext;
        private readonly ZnakerContext _znakerContext;

        public AdJobsService(GrabberContext grabberContext, ZnakerContext znakerContext)
        {
            _grabberContext = grabberContext;
            _znakerContext = znakerContext;
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
            foreach (var jobDemandEntry in jobDemand)
            {
                var sourceTypeJobs = GetJobsForSourceType(jobDemandEntry.Key, jobDemandEntry.Value);
                if (sourceTypeJobs.Count > 0)
                {
                    jobDemandResult[jobDemandEntry.Key] = sourceTypeJobs;
                }
            }
            return jobDemandResult;
        }

        public List<AdDownloadJob> GetJobsForSourceType(SourceType sourceType, int count = 1)
        {
            return count < 1
                ? new List<AdDownloadJob>()
                : _grabberContext.AdDownloadJobs.Where(adj => adj.SourceType == sourceType && !adj.ProcessedAt.HasValue)
                    .Take(count)
                    .ToList();
        }

        public void SaveJobResult(AdJobResult jobResult)
        {
            _grabberContext.AdDownloadJobs.First(adj => adj.Id == jobResult.DownloadJob.Id).ProcessedAt = DateTime.Now;
            _grabberContext.SaveChanges();
            Entry entry;
            try
            {
                entry = _znakerContext.Entries.First(
                    e => e.IdOnSource == jobResult.DownloadJob.AdId && e.SourceId == jobResult.DownloadJob.SourceType);
            }
            catch (InvalidOperationException e)
            {
                entry = new Entry
                {
                    Text = jobResult.Text,
                    SourceId = jobResult.DownloadJob.SourceType,
                    CreatedOn = DateTime.Now,
                    IdOnSource = jobResult.DownloadJob.AdId
                };
                _znakerContext.Add(entry);
            }
            if (jobResult.Contacts == null || jobResult.Contacts.Count == 0)
            {
                _znakerContext.SaveChanges();
                return;
            }
            var contacts = jobResult.Contacts.Select(k =>
            {
                var contact =
                    _znakerContext.Contacts.FirstOrDefault(c => c.ContactType == k.Key && c.Identity == k.Value);
                if (null != contact)
                {
                    contact.UpdatedOn = DateTime.Now;
                    return contact;
                }
                contact = new Contact
                {
                    ContactType = k.Key,
                    Identity = k.Value,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now
                };
                _znakerContext.Contacts.Add(contact);
                return contact;
            }).ToList();
            var entryContacts = contacts.Select(c => new EntryContact
            {
                Contact = c,
                Entry = entry
            }).ToList();
            _znakerContext.AddRange(entryContacts);
            _znakerContext.SaveChanges();
        }
    }
}