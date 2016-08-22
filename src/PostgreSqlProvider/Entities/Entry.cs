using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostgreSqlProvider.Entities
{
    /**
     * Объявление
     */
    public class Entry
    {
        public long Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<Phone> Phones { get; set; } = new List<Phone>();
        public List<MessengerContact> Messenger { get; set; } = new List<MessengerContact>();
        public List<Email> Emails { get; set; } = new List<Email>();
    }
}
