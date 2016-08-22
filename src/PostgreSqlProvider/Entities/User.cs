using System;
using System.Collections.Generic;

namespace PostgreSqlProvider.Entities
{
    public class User
    {
        public long Id { get; set; }
        public DateTime CreateDateTime { get; set; }
        public List<UserIdentity> UserIdentities { get; set; } = new List<UserIdentity>();
        public List<UserData> UserIdentitiesDatas { get; set; } = new List<UserData>();
    }
}

