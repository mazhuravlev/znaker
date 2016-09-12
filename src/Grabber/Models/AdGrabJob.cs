using System;
using Infrastructure;

namespace Grabber.Models
{
    public class AdGrabJob
    {
        public long Id { get; set; }
        public SourceType SourceType { get; set; }
        public string AdId { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}