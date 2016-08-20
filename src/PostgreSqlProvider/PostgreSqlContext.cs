using DomainModel.Entities;
using Microsoft.EntityFrameworkCore;

namespace PostgreSqlProvider
{
    public class PostgreSqlContext : DbContext
    {
        public PostgreSqlContext(DbContextOptions<PostgreSqlContext> options) : base(options) { }

        public DbSet<TestEntity> TestEntities { get; set; }
        public DbSet<Phone> Phones { get; set; }


        protected override void OnModelCreating(ModelBuilder b)
        {
        }
    }
}
