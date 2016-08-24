using System.Collections.Generic;

namespace PostgreSqlProvider.Entities
{
    public class Source
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Entry> Entries { get; set; } = new List<Entry>();
    }
}