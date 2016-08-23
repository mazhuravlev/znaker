using System;
using System.Collections.Generic;

namespace PostgreSqlProvider.Entities.v1
{
    public class Service
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTime CreateDateTime { get; set; }
        public List<UserData> UserDatas { get; set; } = new List<UserData>();
    }
}

