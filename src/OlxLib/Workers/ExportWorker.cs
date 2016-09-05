using System;
using System.Collections.Generic;
using Infrastructure;
using OlxLib.Entities;
using PostgreSqlProvider;
using PostgreSqlProvider.Entities;
using System.Linq;
using PhoneUtils;
using PhoneUtils.CountryRules;

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
            if (null == exportJob.DownloadJob)
            {
                throw new Exception("Download job is null");
            }
            var entry = new Entry
            {
                Text = exportJob.Data.Text,
                SourceId = (SourceType) exportJob.DownloadJob.OlxType,
                CreatedOn = DateTime.Now,
                IdOnSource = exportJob.DownloadJob.AdvId.ToString()
            };
            _znakerContext.Add(entry);
            if (exportJob.Data.Contacts == null) return entry;
            var normalizer = new PhoneNormalizer(GetCountryRules(exportJob.DownloadJob.OlxType));
            var contacts = GetContacts(exportJob.Data.Contacts, normalizer);
            var entryContacts = contacts.Select(c => new EntryContact
            {
                Contact = c,
                Entry = entry
            }).ToList();
            _znakerContext.AddRange(entryContacts);
            entry.EntryContacts = entryContacts;
            return entry;
        }

        private AbstractCountryRule GetCountryRules(OlxType olxType)
        {
            switch (olxType)
            {
                case OlxType.Ua:
                    return new UaCountryRule();
                case OlxType.By:
                    return new ByCountryRule();
                case OlxType.Uz:
                    return new UzCountryRule();
                case OlxType.Kz:
                    return new KzCountryRule();
                default:
                    throw new ArgumentOutOfRangeException(nameof(olxType), olxType, null);
            }
        }

        private IEnumerable<Contact> GetContacts(List<KeyValuePair<ContactType, string>> contactData, PhoneNormalizer normalizer)
        {
            var contacts = new List<Contact>();
            foreach (var k in contactData)
            {
                var contact =
                    _znakerContext.Contacts.FirstOrDefault(c => c.ContactType == k.Key && c.Identity == k.Value);
                if (null != contact)
                {
                    contact.UpdatedOn = DateTime.Now;
                    contacts.Add(contact);
                }
                else
                {
                    if (!k.Value.HasText())
                    {
                        continue;
                    }
                    string identity;
                    try
                    {
                        identity = k.Key == ContactType.Phone ? normalizer.Normalize(k.Value) : k.Value;
                    }
                    catch (PhoneNormalizationException)
                    {
                        // phone is not recognized
                        // TODO: Log or save original contact
                        continue;
                    }
                    contact = new Contact
                    {
                        ContactType = k.Key,
                        Identity = identity,
                        CreatedOn = DateTime.Now,
                        UpdatedOn = DateTime.Now
                    };
                    _znakerContext.Contacts.Add(contact);
                    contacts.Add(contact);
                }
            }
            return contacts;
        }
    }
}