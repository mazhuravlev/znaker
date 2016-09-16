﻿using System;
using Infrastructure;
using PostgreSqlProvider.Entities;

namespace Grabber2.Entities
{
    public class AdDownloadJob
    {
        public long Id { get; set; }
        public SourceType SourceType { get; set; }
        public string AdId { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}