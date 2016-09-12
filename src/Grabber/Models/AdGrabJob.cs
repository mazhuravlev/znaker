using System;
using Infrastructure;

namespace Grabber.Models
{
    public class AdGrabJob
    {
        public SourceType SourceType { get; set; }
        public string AdId { get; set; }
    }
}