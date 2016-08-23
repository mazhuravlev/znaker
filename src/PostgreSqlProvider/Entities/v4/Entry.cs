using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostgreSqlProvider.Entities.v4
{
    public class Entry
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedOn { get; set; }
        public Source Source { get; set; }

        [InverseProperty("Entry")]
        public List<EntryContact> EntryContacts { get; set; } = new List<EntryContact>();
    }
}