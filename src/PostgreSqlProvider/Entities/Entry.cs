using System;
using System.Collections.Generic;

namespace PostgreSqlProvider.Entities
{
    public class Entry
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedOn { get; set; }
        public int SourceId { get; set; }
        public Source Source { get; set; }
        public string IdOnSource { get; set; }
        public List<EntryContact> EntryContacts { get; set; } = new List<EntryContact>();
    }
}