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
        private readonly HttpClient _client;

        public DownloadWorker(HttpClient client)
        {
            _client = client;
        }

        public OlxDownloadResult Run(DownloadJob downloadJob)
        {
            var config = BaseWorker.GetOlxConfig(downloadJob.OlxType);

            var downloadResult = new OlxDownloadResult
            {
                DownloadJob = downloadJob,
                OlxAdvert = null,
            };
            var adResponse = _client.GetAsync(config.GetAdvertDataUrl(downloadJob.AdvId)).Result;
            downloadJob.AdHttpStatusCode = adResponse.StatusCode;
            downloadJob.UpdatedAt = DateTime.Now;
            if (adResponse.IsSuccessStatusCode)
            {
                downloadResult.OlxAdvert = new OlxAdvert
                {
                    Text = ExtractAdTextFromJsonString(adResponse.Content.ReadAsStringAsync().Result)
                };
            }
            else
            {
                return downloadResult;
            }
            var contactsResponse = _client.GetAsync(config.GetAdvertContactUrl(downloadJob.AdvId)).Result;
            downloadJob.ContactsHttpStatusCode = contactsResponse.StatusCode;
            if (contactsResponse.IsSuccessStatusCode)
            {
                downloadResult.OlxAdvert.Contacts = ExtractAdContactsFromJsonString(contactsResponse.Content.ReadAsStringAsync().Result);
            }

            downloadJob.ProcessedAt = DateTime.Now;

            return downloadResult;
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
                .Select(jt => (string) jt["uri"])
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