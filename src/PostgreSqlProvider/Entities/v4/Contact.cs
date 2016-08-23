using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostgreSqlProvider.Entities.v4
{
    public class Contact
    {
        [Key, Column(Order = 1)]
        public string Id { get; set; }
        [Key, Column(Order = 0)]
        public ContactType ContactType { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<Entry> Entry { get; set; } = new List<Entry>();
    }
}