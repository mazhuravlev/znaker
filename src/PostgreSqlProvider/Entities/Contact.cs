using System;
using System.Collections.Generic;
using Infrastructure;

namespace PostgreSqlProvider.Entities
{ 
    public class Contact
    {
        public long Id { get; set; }
        public string Identity { get; set; }
        public ContactType ContactType { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public List<EntryContact> EntryContacts { get; set; } = new List<EntryContact>();
    }
}