using Microsoft.EntityFrameworkCore;
using OlxLib.Entities;

namespace OlxLib
{
    public class ParserContext : DbContext
    {
        public ParserContext(DbContextOptions<ParserContext> options) : base(options) { }


        public DbSet<Test> Tests { get; set; }



        protected override void OnModelCreating(ModelBuilder b)
        {



        }
    }
}
