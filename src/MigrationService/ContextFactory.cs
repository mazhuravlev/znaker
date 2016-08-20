using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using PostgreSqlProvider;

namespace MigrationService
{
    public class ContextFactory : IDbContextFactory<PostgreSqlContext>
    {
        public PostgreSqlContext Create(DbContextFactoryOptions options)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", false).Build();
            var builder = new DbContextOptionsBuilder<PostgreSqlContext>();
            builder.UseNpgsql(configuration[$"ConnectionString:{options.EnvironmentName}"], b => b.MigrationsAssembly("MigrationService"));
            return new PostgreSqlContext(builder.Options);
        }
    }
}
