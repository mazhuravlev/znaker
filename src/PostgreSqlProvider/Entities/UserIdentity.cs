using System;
using System.Collections.Generic;

namespace PostgreSqlProvider.Entities
{
    public class UserIdentity
    {
        public string Id { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
        public DateTime CreateDateTime { get; set; }
        public List<UserData> UserDatas { get; set; } = new List<UserData>();
    }
}

