using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostgreSqlProvider.Entities.v3
{
    //Объявление, один источник(площадка), много контактов
    public class Entry
    {
        public long Id { get; set; }
        public DateTime CreateDateTime { get; set; }
        public Source Source { get; set; }
        public long SourceId { get; set; }
        public List<EntryInContact> EntryContacts { get; set; } = new List<EntryInContact>();
    }
}
