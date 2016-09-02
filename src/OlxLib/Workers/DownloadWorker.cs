using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OlxLib.Entities;
using OlxLib.Utils;

namespace OlxLib.Workers
{
    public class DownloadWorker
    {
        private readonly ParserContext _db;
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
            return _db.DownloadJobs.First(dj => dj.ProcessedAt == null);
        }

        private void ProcessJob(DownloadJob downloadJob)
        {
            downloadJob.UpdatedAt = DateTime.Now;
            var client = new HttpClient();
            var adResponse = client.GetAsync(_config.GetAdvertDataUrl(downloadJob.AdvId)).Result;
            var contactsResponseTask = client.GetAsync(_config.GetAdvertContactUrl(downloadJob.AdvId));
            downloadJob.AdHttpStatusCode = adResponse.StatusCode;
            if (!adResponse.IsSuccessStatusCode) return;
            var adText = ExtractAdTextFromJsonString(adResponse.Content.ReadAsStringAsync().Result);
            var exportJob = new ExportJob
            {
                DownloadJob = downloadJob,
                Data = new OlxAdvert
                {
                    Text = adText
                }
            };
            _db.Add(exportJob);
            var contactsResponse = contactsResponseTask.Result;
            downloadJob.ContactsHttpStatusCode = contactsResponse.StatusCode;
            if (!contactsResponse.IsSuccessStatusCode) return;
            var contacts = ExtractAdContactsFromJsonString(contactsResponse.Content.ReadAsStringAsync().Result);
            if (null == contacts) return;
            exportJob.Data.Contacts = contacts;
            downloadJob.ProcessedAt = DateTime.Now;
        }

        private static List<KeyValuePair<ContactType, string>> ExtractAdContactsFromJsonString(string contactsJsonString)
        {
            JObject jObject;
            try
            {
                jObject = JsonConvert.DeserializeObject<JObject>(contactsJsonString);
            }
            catch (JsonReaderException)
            {
                // invalid json
                return null;
            }
            JToken phoneJToken;
            try
            {
                phoneJToken = jObject["urls"]["phone"];
            }
            catch (NullReferenceException)
            {
                // json doesn't have required fields
                return null;
            }
            List<string> phoneStringList;
            switch (phoneJToken.Type)
            {
                case JTokenType.Array:
                    phoneStringList = ExtractPhonesFromJson(phoneJToken.Children());
                    break;
                case JTokenType.Object:
                    phoneStringList = ExtractPhonesFromJson(new List<JToken> {phoneJToken});
                    break;
                default:
                    throw new Exception("Unknown type: " + jObject.Type);
            }
            return phoneStringList
                .Select(s => new KeyValuePair<ContactType, string>(ContactType.Phone, s))
                .ToList();
        }

        private static List<string> ExtractPhonesFromJson(IEnumerable<JToken> phoneJtokenList)
        {
            return phoneJtokenList
                .Select(jt => PhoneNormalizer.Normalize((string) jt["uri"]))
                .ToList();
        }

        private static string ExtractAdTextFromJsonString(string adJsonString)
        {
            JObject jObject;
            try
            {
                jObject = JsonConvert.DeserializeObject<JObject>(adJsonString);
            }
            catch (JsonReaderException)
            {
                // invalid json
                return null;
            }
            string text;
            try
            {
                text = (string) jObject["ad"]["title"] + ", " + (string) jObject["ad"]["description"];
            }
            catch (NullReferenceException)
            {
                // json doesn't have required fields
                return null;
            }
            return TextUtils.CleanSpacesAndNewlines(text);
        }
    }
}