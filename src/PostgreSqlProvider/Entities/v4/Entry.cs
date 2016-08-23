using System;
using System.Collections.Generic;

namespace PostgreSqlProvider.Entities.v4
{
    public class Entry
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedOn { get; set; }
        public Source Source { get; set; }
        public List<Contact> Contact { get; set; } = new List<Contact>();
    }
}
