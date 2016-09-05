using System;
using System.Linq;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OlxLib.Workers;
using Xunit;

namespace OlxLib.Test
{
    public class DownloadWorkerTests
    {
        [Fact]
        public void TestRun()
        {
            var parserContex = CreateServiceProvider().GetService<ParserContext>();
            var worker = new DownloadWorker(new HttpClient());
            var downloadJob =
                parserContex.DownloadJobs.Skip(5)
                    .FirstOrDefault(dj => dj.OlxType == OlxType.Ua && !dj.ProcessedAt.HasValue);
            var downloadResult = worker.Run(downloadJob.AdvId, downloadJob.OlxType);
            Console.WriteLine("OK!");
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