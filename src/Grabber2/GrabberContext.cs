using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabber2.Entities;
using Microsoft.EntityFrameworkCore;

namespace Grabber2 { 
    public class GrabberContext : DbContext
    {
        public DbSet<SitemapEntry> SitemapEntries { get; set; }
        public DbSet<AdDownloadJob> AdDownloadJobs { get; set; }

        public GrabberContext(DbContextOptions<GrabberContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<SitemapEntry>().HasKey(se => se.Id);
            b.Entity<AdDownloadJob>().HasKey(adj => adj.Id);
        }
    }
}
