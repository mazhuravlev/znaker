using System;

namespace PostgreSqlProvider.Entities.v2
{
    public class MessengerContact
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Type { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}

