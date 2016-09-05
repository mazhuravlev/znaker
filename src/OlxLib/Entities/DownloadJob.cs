using System;
using System.Net;

namespace OlxLib.Entities
{
    public class DownloadJob
    {
        public int Id { get; set; }
        public OlxType OlxType { get; set; }
        public int AdvId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public HttpStatusCode? AdHttpStatusCode { get; set; }
        public HttpStatusCode? ContactsHttpStatusCode { get; set; }
        public ExportJob ExportJob { get; set; }
    }
}