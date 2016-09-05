using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public OlxDownloadResult Run(int advId, OlxType olxType)
        {
            var config = BaseWorker.GetOlxConfig(olxType);

            var result = new OlxDownloadResult
            {
                AdvId = advId,
                OlxAdvert = null
            };
            var adResponse = _client.GetAsync(config.GetAdvertDataUrl(advId)).Result;
            result.AdHttpStatusCode = adResponse.StatusCode;
            if (adResponse.IsSuccessStatusCode)
            {
                result.OlxAdvert = new OlxAdvert
                {
                    Text = ExtractAdTextFromJsonString(adResponse.Content.ReadAsStringAsync().Result)
                };
            }
            else
            {
                if (adResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    result.ProcessedAt = DateTime.Now;
                }
                return result;
            }
            var contactsResponse = _client.GetAsync(config.GetAdvertContactUrl(advId)).Result;
            result.ContactsHttpStatusCode = contactsResponse.StatusCode;
            if (contactsResponse.IsSuccessStatusCode)
            {
                result.OlxAdvert.Contacts = ExtractAdContactsFromJsonString(contactsResponse.Content.ReadAsStringAsync().Result);
            }
            result.ProcessedAt = DateTime.Now;
            return result;
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