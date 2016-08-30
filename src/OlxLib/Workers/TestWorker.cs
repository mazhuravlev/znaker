using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using OlxLib.Entities;

namespace OlxLib.Workers
{
    public class TestWorker : IDisposable
    {
        private readonly ParserContext _db;

        public TestWorker(ParserContext db)
        {
            _db = db;
        }

        [AutomaticRetry(Attempts = 0)]
        public void Run()
        {
            _db.Tests.Add(new Test
            {
                CreateAt = DateTime.Now
            });
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
