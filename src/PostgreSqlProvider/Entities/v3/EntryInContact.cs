using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostgreSqlProvider.Entities.v3
{
    //многие ко многим
    public class EntryInContact
    {
        public Entry Entry { get; set; }
        public long EntryId { get; set; }
        public Contact Contact { get; set; }
        public long ContactId { get; set; }
    }
}
