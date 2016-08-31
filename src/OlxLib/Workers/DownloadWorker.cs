using System;
using System.Linq;
using Hangfire.Server;

namespace OlxLib.Workers
{
    public class DownloadWorker : BaseWorker
    {
        public DownloadWorker(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public string Run()
        {
            using (var db = GetZnakerContext())
            {
                var r = db.Contacts.Count();
            }
            return "OK!";
        }

        
    }
}
