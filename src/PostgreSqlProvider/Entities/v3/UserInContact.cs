using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostgreSqlProvider.Entities.v3
{
    //многие ко многим
    public class UserInContact
    {
        public User User { get; set; }
        public long UserId { get; set; }
        public Contact Contact { get; set; }
        public long ContactId { get; set; }
    }
}
