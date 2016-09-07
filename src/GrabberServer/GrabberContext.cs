using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrabberServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace GrabberServer
{
    public class GrabberContext : DbContext
    {
        public DbSet<SitemapEntry> SitemapEntries { get; set; }
        public GrabberContext(DbContextOptions<GrabberContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<SitemapEntry>(
                //c => c
            );
        }
    }
}
