using Microsoft.EntityFrameworkCore;
using PostgreSqlProvider.Entities;

namespace PostgreSqlProvider
{
    public class PostgreSqlContext : DbContext
    {
        public PostgreSqlContext(DbContextOptions<PostgreSqlContext> options) : base(options)
        {
        }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<EntryContact> EntryContacts { get; set; }


        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Contact>().HasIndex(c => c.Identity).IsUnique(false);
            b.Entity<Contact>().HasIndex(c => new {c.ContactType, c.Identity}).IsUnique();
            b.Entity<Contact>().Property(c => c.Identity).IsRequired().HasColumnType("Varchar(40)");

            b.Entity<EntryContact>()
                .HasOne(pt => pt.Contact)
                .WithMany(p => p.EntryContacts)
                .HasForeignKey(pt => pt.ContactId);

            b.Entity<EntryContact>()
                .HasOne(pt => pt.Entry)
                .WithMany(t => t.EntryContacts)
                .HasForeignKey(pt => pt.EntryId);
        }
    }
}