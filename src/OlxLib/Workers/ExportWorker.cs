using System;
using System.Linq;
using Infrastructure;
using OlxLib.Entities;
using PostgreSqlProvider;
using PostgreSqlProvider.Entities;

namespace OlxLib.Workers
{
    public class ExportWorker
    {
        private readonly ZnakerContext _db;
        public ExportWorker(ZnakerContext db)
        {
            _db = db;
        }

        public Entry Run(ExportJob exportJob)
        {
            var entry = new Entry();
            _db.Add(entry);
            if (null != exportJob.Data.Contacts)
            {
                var contacts = exportJob.Data.Contacts.Select(k =>
                {
                    var contact = _db.Contacts.FirstOrDefault(c => c.ContactType == k.Key && c.Identity == k.Value);
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
                    _db.Contacts.Add(contact);
                    return contact;
                }).ToList();
                var entryContacts = contacts.Select(c => new EntryContact
                {
                    Contact = c,
                    Entry = entry
                }).ToList();
                _db.AddRange(entryContacts);
                entry.EntryContacts = entryContacts;
            }
            entry.Text = exportJob.Data.Text;
            entry.SourceId = (SourceType) exportJob.DownloadJob.OlxType;
            entry.CreatedOn = DateTime.Now;
            entry.IdOnSource = exportJob.DownloadJob.AdvId.ToString();
            return entry;
        }
    }
}
