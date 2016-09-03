using System;
using System.Collections.Generic;
using Infrastructure;

namespace PostgreSqlProvider.Entities
{
    public class Entry
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedOn { get; set; }
        public SourceType SourceId { get; set; }
        public Source Source { get; set; }
        public string IdOnSource { get; set; }
        public List<EntryContact> EntryContacts { get; set; } = new List<EntryContact>();
    }
}