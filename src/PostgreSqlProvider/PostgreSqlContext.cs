using Microsoft.EntityFrameworkCore;
using PostgreSqlProvider.Entities.v4;

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
        }
    }
}