using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostgreSqlProvider.Entities.v3
{
    //источник, например авита
    public class Source
    {
        public long Id { get; set; }
        public DateTime CreateDateTime { get; set; }
        public List<Entry> Entries { get; set; } = new List<Entry>();
    }
}
