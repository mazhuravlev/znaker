using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OlxUa
{
    public class Program
    {
        public static void Main(string[] args)
        {

        }

        public static async Task<HttpStatusCode> GetCode(string url)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(url);
            return response.StatusCode;
        }

        public static void IncrementSearch()
        {
            const string urlTemplate =
                "https://ssl.olx.ua/i2/ajax/ad/getcontact/?type=phone&json=1&id={0}&version=2.3.2";
            var startId = 148486659;
            var offset = 0;
            var okCount = 0;
            var notFountCount = 0;
            while (true)
            {
                var result = GetCode(string.Format(urlTemplate, startId + offset)).Result;
                switch (result)
                {
                    case HttpStatusCode.NotFound:
                        notFountCount++;
                        break;
                    case HttpStatusCode.OK:
                        okCount++;
                        break;
                    default:
                        Console.WriteLine(result.ToString());
                        break;
                }
                offset--;
                Console.WriteLine(string.Format("OK: {0}, NotFound: {1}", okCount, notFountCount));
            }
        }

        public static async Task ListSearch()
        {
            var client = new HttpClient();
            var responseJson = await client.GetStringAsync("https://ssl.olx.ua/i2/ads/?json=1&longlist=1&search");

        }
    }
}
