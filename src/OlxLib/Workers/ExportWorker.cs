using System;
using Infrastructure;
using OlxLib.Entities;
using PostgreSqlProvider;
using PostgreSqlProvider.Entities;
using System.Linq;

namespace OlxLib.Workers
{
    public class ExportWorker
    {
        private readonly ZnakerContext _znakerContext;
        public ExportWorker(ZnakerContext znakerContext)
        {
            _znakerContext = znakerContext;
        }

        public Entry Run(ExportJob exportJob)
        {
            var entry = new Entry
            {
                Text = exportJob.Data.Text,
                SourceId = (SourceType)exportJob.DownloadJob.OlxType,
                CreatedOn = DateTime.Now,
                IdOnSource = exportJob.DownloadJob.AdvId.ToString()
            };
            _znakerContext.Add(entry);
            if (exportJob.Data.Contacts == null) return entry;
            var contacts = exportJob.Data.Contacts.Select(k =>
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
            return entry;
        }
    }
}
