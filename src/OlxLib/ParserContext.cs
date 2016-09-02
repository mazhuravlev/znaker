using System;
using Microsoft.EntityFrameworkCore;
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
            b.Entity<DownloadJob>(
                dj => dj.HasIndex(c => new {c.OlxType, c.AdvId}).IsUnique()
            );
            b.Entity<DownloadJob>().Property(dj => dj.CreatedAt).HasDefaultValue(DateTime.Now);
        }
    }
}
