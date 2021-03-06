﻿using System;
using Microsoft.EntityFrameworkCore;
using PostgreSqlProvider.Entities;

namespace PostgreSqlProvider
{
    public class ZnakerContext : DbContext
    {
        public ZnakerContext(DbContextOptions<ZnakerContext> options) : base(options)
        {
        }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<EntryContact> EntryContacts { get; set; }


        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Contact>(e =>
            {
                e.HasIndex(c => c.Identity).IsUnique(false);
                e.HasIndex(c => new { c.ContactType, c.Identity }).IsUnique();
                e.Property(c => c.Identity).IsRequired().HasColumnType("Varchar(40)");
            });
            b.Entity<Contact>().Property(c => c.CreatedOn).HasDefaultValue(DateTime.Now);
            b.Entity<Contact>().Property(c => c.UpdatedOn).HasDefaultValue(DateTime.Now);
            
            b.Entity<Entry>(e =>
            {
                e.Property(c => c.IdOnSource).IsRequired().HasColumnType("Varchar(32)");
                e.HasIndex(c => new { c.SourceId, c.IdOnSource }).IsUnique();
            });
            b.Entity<Entry>().Property(e => e.CreatedOn).HasDefaultValue(DateTime.Now);

            b.Entity<EntryContact>(e =>
            {
                e.HasOne(pt => pt.Contact).WithMany(p => p.EntryContacts).HasForeignKey(pt => pt.ContactId);
                e.HasOne(pt => pt.Entry).WithMany(t => t.EntryContacts).HasForeignKey(pt => pt.EntryId);
            });

            b.Entity<Source>(e =>
            {
                e.Property(c => c.Id).ValueGeneratedNever();
            });
        }
    }
}