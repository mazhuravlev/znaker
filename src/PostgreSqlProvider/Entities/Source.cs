using System.Collections.Generic;
using Infrastructure;

namespace PostgreSqlProvider.Entities
{
    public class Source
    {
        public SourceType Id { get; set; }
        public string Title { get; set; }
        public string SiteUrl { get; set; }
        public List<Entry> Entries { get; set; } = new List<Entry>();
    }
}