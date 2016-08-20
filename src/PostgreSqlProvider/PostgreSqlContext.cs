using Microsoft.EntityFrameworkCore;
using PostgreSqlProvider.Entities;

namespace PostgreSqlProvider
{
    public class PostgreSqlContext : DbContext
    {
        public PostgreSqlContext(DbContextOptions<PostgreSqlContext> options) : base(options) { }

        public DbSet<Phone> Phones { get; set; }


        protected override void OnModelCreating(ModelBuilder b)
        {
        }
    }
}
