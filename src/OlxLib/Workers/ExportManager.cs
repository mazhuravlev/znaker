using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using PostgreSqlProvider;
using System.Linq;
using Microsoft.EntityFrameworkCore;


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

        [Queue("Export manager")]
        public void Run(IJobCancellationToken cancellationToken)
        {
            var cts = new CancellationTokenSource();
            var task = Export(cts.Token);
            while (true)
            {
                Thread.Sleep(1000);
                if (cancellationToken == null || !cancellationToken.ShutdownToken.IsCancellationRequested) continue;
                cts.Cancel();
                Task.WaitAll(task);
                return;
            }
        }

        private Task Export(CancellationToken cancellationToken)
        {
            var exportWorker = new ExportWorker(_znakerContext);
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return Task.CompletedTask;
                }
                var exportJob = _parserContext
                    .ExportJobs
                    .Include(ej => ej.DownloadJob)
                    .First(ej => null == ej.ExportedAt);
                var entry = exportWorker.Run(exportJob);
                _znakerContext.SaveChanges();
            }
        }
    }
}