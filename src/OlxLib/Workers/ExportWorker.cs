using System;
using Hangfire;
using PostgreSqlProvider;
using System.Linq;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using PostgreSqlProvider.Entities;


namespace OlxLib.Workers
{
    public class ExportWorker
    {
        private readonly ZnakerContext _znakerContext;
        private readonly ParserContext _parserContext;

        public ExportWorker(ZnakerContext znakerContext, ParserContext parserContext)
        {
            _znakerContext = znakerContext;
            _parserContext = parserContext;
        }

        [Queue("Export manager")]
        [DisableConcurrentExecution(600)]
        public string RunExport(IJobCancellationToken cancellationToken, int exportLimit)
        {
            var expoted = 0;
            var exportJobs = _parserContext
                .ExportJobs
                .Include(ej => ej.DownloadJob)
                .Where(ej => !ej.ExportedAt.HasValue)
                .Take(exportLimit)
                .ToList();

            foreach (var job in exportJobs)
            {
                if (cancellationToken != null && cancellationToken.ShutdownToken.IsCancellationRequested)
                {
                    break;
                }

                var entry = new Entry
                {
                    Text = job.Data.Text,
                    SourceId = (SourceType) job.DownloadJob.OlxType,
                    CreatedOn = DateTime.Now,
                    IdOnSource = job.DownloadJob.AdvId.ToString()
                };
                _znakerContext.Add(entry);

                if (job.Data.Contacts != null)
                {
                    var contacts = job.Data.Contacts.Select(k =>
                    {
                        var contact = _znakerContext.Contacts.FirstOrDefault(c => c.ContactType == k.Key && c.Identity == k.Value);
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
                    entry.EntryContacts = entryContacts;
                }
                
                _znakerContext.SaveChanges();
                _parserContext.SaveChanges();
                expoted++;
            }
            return $"exported {expoted}/{exportLimit}";
        }

        [Queue("Export manager")]
        public string RunCleaner(int olderThanDays)
        {
            return _parserContext.Database.ExecuteSqlCommand($"DELETE FROM public.\"ExportJobs\" WHERE \"CreateAt\" < '{DateTime.Now.AddDays(-olderThanDays):yyyy-MM-dd HH:mm:ss}'").ToString();
        }
    }
}