using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Hangfire.Server;
using Hangfire.Storage.Monitoring;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OlxLib.Entities;

namespace OlxLib.Workers
{
    public class DownloadWorker
    {
        private ParserContext _db;
        private OlxConfig _config;

        public DownloadWorker(ParserContext db)
        {
            _db = db;
        }

        public string Run(OlxType olxType)
        {
            _config = BaseWorker.GetOlxConfig(olxType);

            ProcessJob(GetNextJob());
            _db.SaveChanges();

            return "Ok!";
        }

        private DownloadJob GetNextJob()
        {
            return _db.DownloadJobs.First(dj => dj.OlxResponse != OlxResponse.Ok);
        }

        private void ProcessJob(DownloadJob downloadJob)
        {
            downloadJob.UpdatedAt = DateTime.Now;
            var client = new HttpClient();
            var adResponse = client.GetAsync(_config.GetAdvertDataUrl(downloadJob.AdvId)).Result;
            var contactsResponseTask = client.GetAsync(_config.GetAdvertContactUrl(downloadJob.AdvId));
            downloadJob.AdHttpStatusCode = adResponse.StatusCode;
            if (!adResponse.IsSuccessStatusCode) return;
            var exportJob = new ExportJob
            {
                DownloadJob = downloadJob,
                Data = new OlxAdvert
                {
                    Text = (string)JsonConvert.DeserializeObject<JObject>(adResponse.Content.ReadAsStringAsync().Result)["ad"]["description"]
                }
            };
            _db.Add(exportJob);
            var contactsResponse = contactsResponseTask.Result;
            downloadJob.ContactsHttpStatusCode = contactsResponse.StatusCode;
            downloadJob.ProcessedAt = DateTime.Now;
        }
    }
}