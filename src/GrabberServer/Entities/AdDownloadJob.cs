using Infrastructure;
using PostgreSqlProvider.Entities;

namespace GrabberServer.Entities
{
    public class AdDownloadJob
    {
        public long Id;
        public SourceType SourceType;
        public string AdId { get; set; }
    }
}