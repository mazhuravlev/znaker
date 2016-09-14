using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Grabber.Infrastructure;
using Grabber.Models;
using Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Grabber.Grabbers.Olx
{
    public class OlxAdvertGrabber : IAdvertGrabber
    {
        private readonly OlxConfig _config;
        private readonly IHttpClient _client;

        public OlxAdvertGrabber(OlxConfig config, IHttpClient client)
        {
            _config = config;
            _client = client;
        }

        public AdvertJobResult Process(AdvertJob job)
        {
            var result = new AdvertJobResult
            {
                Job = job
            };
            var adResponse = _client.GetAsync(_config.GetAdvertDataUrl(job.Id)).Result;
            if (adResponse.IsSuccessStatusCode)
            {
                result.Text = ExtractAdTextFromJsonString(adResponse.Content.ReadAsStringAsync().Result);
            }
            else
            {
                throw new HttpRequestException("Response code: " + adResponse.StatusCode);
            }
            var contactsResponse = _client.GetAsync(_config.GetAdvertContactUrl(job.Id)).Result;
            if (contactsResponse.IsSuccessStatusCode)
            {
                result.Contacts = ExtractAdContactsFromJsonString(contactsResponse.Content.ReadAsStringAsync().Result);
            }
            return result;
        }

        public SourceType GetSourceType()
        {
            return (SourceType) _config.OlxType;
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
            return phoneStringList.Select(s => new KeyValuePair<ContactType, string>(ContactType.Phone, s)).ToList();
        }

        private static List<string> ExtractPhonesFromJson(IEnumerable<JToken> phoneJtokenList)
        {
            return phoneJtokenList.Select(jt => (string) jt["uri"]).ToList();
        }

        private static string ExtractAdTextFromJsonString(string adJsonString)
        {
            var jObject = JsonConvert.DeserializeObject<JObject>(adJsonString);
            var   text = (string) jObject["ad"]["title"] + ", " + (string) jObject["ad"]["description"];
            return TextUtils.CleanSpacesAndNewlines(text);
        }
    }
}