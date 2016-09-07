using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NuGet.Configuration;

namespace GrabberServer.Grabbers.Nadproxy
{
    public class SimpleNadproxy : IGrabberHttpClient
    {
        private readonly List<HttpClient> _httpClients;
        private int _counter = 0;

        public SimpleNadproxy(IWebProxy webProxy)
        {
            _httpClients = new List<HttpClient>
            {
                new HttpClient(new HttpClientHandler
                {
                    Proxy = webProxy
                })
            };
        }

        public SimpleNadproxy(IEnumerable<IWebProxy> webProxies)
        {
            _httpClients = webProxies.Select(
                webProxy => new HttpClient(new HttpClientHandler
                {
                    Proxy = webProxy
                })
            ).ToList();
        }

        public Task<HttpResponseMessage> GetAsync(string url)
        {
            return _httpClients[_counter++ % _httpClients.Count].GetAsync(url);
        }
    }
}