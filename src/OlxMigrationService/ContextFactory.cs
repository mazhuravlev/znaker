using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using OlxLib;

namespace OlxMigrationService
{
    public class ContextFactory : IDbContextFactory<ParserContext>
    {
        public ParserContext Create(DbContextFactoryOptions options)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", false).Build();
            var builder = new DbContextOptionsBuilder<ParserContext>();
            builder.UseNpgsql(configuration[$"ConnectionString:{options.EnvironmentName}"], b => b.MigrationsAssembly("OlxMigrationService"));
            return new ParserContext(builder.Options);
        }
    }
}
