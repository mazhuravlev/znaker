using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OlxLib.Workers;
using Xunit;

namespace OlxLib.Test
{
    public class SitemapWorkerTests
    {
        [Fact]
        public void TestSitemapWorker()
        {
            var worker = new SitemapWorker(CreateServiceProvider().GetService<ParserContext>());
            worker.Run(OlxType.Ua);
        }

        private static IServiceProvider CreateServiceProvider()
        {
            return new ServiceCollection()
                .AddDbContext<ParserContext>(
                    c => c.UseNpgsql(
                        "User ID=dev;Password=dev;Host=localhost;Port=5432;Database=olx_parser;Pooling=true;"),
                    ServiceLifetime.Transient
                )
                .BuildServiceProvider();
        }
    }
}