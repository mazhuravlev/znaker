using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OlxLib.Workers;

namespace OlxLib.Entities
{
    public class DownloadJob
    {
        public int Id { get; set; }
        public OlxType OlxType { get; set; }
        public int AdvId { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public int? HttpResponse { get; set; }
        public OlxResponse OlxResponse { get; set; }
    }
}
