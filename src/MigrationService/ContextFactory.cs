using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using PostgreSqlProvider;

namespace MigrationService
{
    public class ContextFactory : IDbContextFactory<ZnakerContext>
    {
        public ZnakerContext Create(DbContextFactoryOptions options)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", false).Build();
            var builder = new DbContextOptionsBuilder<ZnakerContext>();
            builder.UseNpgsql(configuration[$"ConnectionString:{options.EnvironmentName}"], b => b.MigrationsAssembly("MigrationService"));
            return new ZnakerContext(builder.Options);
        }
    }
}
