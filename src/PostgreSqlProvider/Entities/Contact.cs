using System;
using System.Collections.Generic;

namespace PostgreSqlProvider.Entities
{ 
    public class Contact
    {
        public long Id { get; set; }
        public string Identity { get; set; }
        public ContactTypes ContactType { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<EntryContact> EntryContacts { get; set; } = new List<EntryContact>();
    }
}