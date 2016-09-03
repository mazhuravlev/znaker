using System;
using System.Linq;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OlxLib.Workers;
using PostgreSqlProvider;
using Xunit;

namespace OlxLib.Test
{
    public class ExportWorkerTests
    {
        [Fact]
        public void TestRun()
        {
            var serviceProvider = CreateServiceProvider();
            var db = serviceProvider.GetService<ZnakerContext>();
            var worker = new ExportWorker(db);
            var exportJob = serviceProvider
                .GetService<ParserContext>()
                .ExportJobs
                .Include(ej => ej.DownloadJob)
                .First(ej => ej.DownloadJob.OlxType == OlxType.Ua && null != ej.Data.Contacts);
            var entry = worker.Run(exportJob);
            Assert.Equal(exportJob.DownloadJob.AdvId.ToString(), entry.IdOnSource);
            db.SaveChanges();
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
                .AddDbContext<ZnakerContext>(
                    c => c.UseNpgsql(
                        "User ID=dev;Password=dev;Host=localhost;Port=5432;Database=znaker;Pooling=true;"),
                    ServiceLifetime.Transient
                )
                .BuildServiceProvider();
        }
    }
}