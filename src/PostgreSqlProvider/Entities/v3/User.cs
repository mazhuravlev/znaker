using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostgreSqlProvider.Entities.v3
{
    public class User
    {
        public long Id { get; set; }
        public DateTime CreateDateTime { get; set; }
        public List<UserInContact> UserContacts { get; set; } = new List<UserInContact>();
    }
}
