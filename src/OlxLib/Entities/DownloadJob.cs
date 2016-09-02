using System;

namespace OlxLib.Entities
{
    public class DownloadJob
    {
        public int Id { get; set; }
        public OlxType OlxType { get; set; }
        public int AdvId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public int? HttpResponseCode { get; set; }
        public OlxResponse OlxResponse { get; set; }
    }
}