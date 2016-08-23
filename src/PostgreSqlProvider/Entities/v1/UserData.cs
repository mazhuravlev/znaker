using System;
using System.Collections.Generic;

namespace PostgreSqlProvider.Entities.v1
{
    public class UserData
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
        public long UserIdentityId { get; set; }
        public UserIdentity UserIdentity { get; set; }
        public long ServiceId { get; set; }
        public Service Service { get; set; }
        public string Text { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}

