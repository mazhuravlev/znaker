using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostgreSqlProvider.Entities.v3
{
    //тип контакта, телефон, емайл, вибер, айсику
    public class ContactType
    {
        public long Id { get; set; }
        public DateTime CreateDateTime { get; set; }
        public List<Contact> Contacts { get; set; } = new List<Contact>();
    }
}
