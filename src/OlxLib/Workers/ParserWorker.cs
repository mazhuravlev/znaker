using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OlxLib.Workers
{
    public class ParserWorker : BaseWorker
    {
        public ParserWorker(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public string Run(OlxType type)
        {
            using (var db = GetZnakerContext())
            {
                return "total: " +  db.Contacts.Count();
            }
        }
    }
}
