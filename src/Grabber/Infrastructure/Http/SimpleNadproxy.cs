using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grabber.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace Grabber.Infrastructure.Http
{
    public class SimpleNadproxy : IHttpClient
    {
        private readonly IProxyService _proxyService;
        private readonly List<HttpClient> _httpClients = new List<HttpClient>();
        private int _counter = 0;
        private const int ProxyLimit = 10;

        public SimpleNadproxy(IProxyService proxyService)
        {
            _proxyService = proxyService;
        }

        public Task<HttpResponseMessage> GetAsync(string url)
        {
            if (_httpClients.Count == 0)
            {
                return Task.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
            }
            if (_httpClients.Count < ProxyLimit)
            {
                RequestProxy();
            }
            return _httpClients[_counter++ % _httpClients.Count].GetAsync(url);
        }

        private void RequestProxy()
        {
            try
            {
                var proxy = _proxyService.GetProxy();
                // TODO: somehow use the proxy
            }
            catch (NoResultException)
            {
                // no proxies available in list
            }
        }
    }
}