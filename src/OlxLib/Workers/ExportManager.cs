using System;
using Hangfire;
using PostgreSqlProvider;
using System.Linq;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using PostgreSqlProvider.Entities;
using Npgsql;

namespace OlxLib.Workers
{
    public class ExportManager
    {
        private readonly ZnakerContext _znakerContext;
        private readonly ParserContext _parserContext;

        public ExportManager(ZnakerContext znakerContext, ParserContext parserContext)
        {
            _znakerContext = znakerContext;
            _parserContext = parserContext;
        }

        [Queue("export_manager")]
        [DisableConcurrentExecution(600)]
        public string RunExport(IJobCancellationToken cancellationToken, int exportLimit)
        {
            var exported = 0;
            var duplicates = 0;
            var exportJobs = _parserContext
                .ExportJobs
                .Include(ej => ej.DownloadJob)
                .Where(ej => !ej.ExportedAt.HasValue)
                .Take(exportLimit)
                .ToList();
            var exportWorker = new ExportWorker(_znakerContext);
            foreach (var job in exportJobs)
            {
                if (cancellationToken != null && cancellationToken.ShutdownToken.IsCancellationRequested)
                {
                    break;
                }
                exportWorker.Run(job);
                try
                {
                    _znakerContext.SaveChanges();
                }
                catch (DbUpdateException e)
                {
                    var exception = e.InnerException as PostgresException;
                    if (exception != null &&
                        exception.SqlState == "23505")
                    {
                        duplicates++;
                    }
                    else
                    {
                        throw;
                    }
                }
                job.ExportedAt = DateTime.Now;
                _parserContext.SaveChanges();
                exported++;
            }
            return $"exported: {exported}/{exportLimit}, duplicates: {duplicates}";
        }

        [Queue("export_manager")]
        public string RunCleaner(int olderThanDays)
        {
            return _parserContext.Database.ExecuteSqlCommand($"DELETE FROM public.\"ExportJobs\" WHERE \"ExportedAt\" < '{DateTime.Now.AddDays(-olderThanDays):yyyy-MM-dd HH:mm:ss}'").ToString();
        }
    }
}