using System;
using System.Net;
using OlxLib.Entities;

namespace OlxLib
{
    public class OlxDownloadResult
    {
        public int JobId;
        public int AdvId;
        public OlxAdvert OlxAdvert;
        public HttpStatusCode? AdHttpStatusCode;
        public HttpStatusCode? ContactsHttpStatusCode;
        
        public DateTime? ProcessedAt;
    }
}
