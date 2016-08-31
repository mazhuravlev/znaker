﻿using Microsoft.EntityFrameworkCore;
using OlxLib.Entities;

namespace OlxLib
{
    public class ParserContext : DbContext
    {
        public ParserContext(DbContextOptions<ParserContext> options) : base(options) { }

        public DbSet<DownloadJob> DownloadJobs { get; set; }
        public DbSet<ExportJob> ExportJobs { get; set; }
        public DbSet<ParserMeta> ParserMeta { get; set; }

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<ParserMeta>(
                pm => pm.HasIndex(c => new { c.OlxType, c.Key }).IsUnique()
            );
        }
    }
}
