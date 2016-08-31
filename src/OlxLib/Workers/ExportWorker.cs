using System;
using System.Linq;
using Hangfire;
using Hangfire.Server;

namespace OlxLib.Workers
{
    public class ExportWorker : BaseWorker
    {
        public ExportWorker(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public string Run(PerformContext context)
        {
            using (var db = GetZnakerContext())
            {
                var r = db.Contacts.Count();
                return "OK! " + r;
            }
        }
    }
}
