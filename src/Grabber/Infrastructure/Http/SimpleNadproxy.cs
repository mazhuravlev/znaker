using System;
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
        private readonly ILogger _logger;
        private readonly List<HttpClient> _httpClients = new List<HttpClient>();
        private int _counter = 0;
        private const int ProxyLimit = 10;

        public SimpleNadproxy(IProxyService proxyService, ILogger<SimpleNadproxy> logger)
        {
            _proxyService = proxyService;
            _logger = logger;
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
            _counter++;
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            return _httpClients[GetProxyNumber()].GetAsync(url, cts.Token);
        }

        private static CancellationToken GetTimeoutToken(int seconds)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(seconds));
            return cts.Token;
        }

        private int GetProxyNumber()
        {
            return _counter%_httpClients.Count;
        }

        public void AddProxy(IWebProxy proxy)
        {
            _httpClients.Add(new HttpClient(new HttpClientHandler
                    {
                        Proxy = proxy,
                        UseProxy = true
                    })
                );
        }

        private void RequestProxy()
        {
            try
            {
                AddProxy(_proxyService.GetProxy());
            }
            catch (NoResultException)
            {
                // no proxies available in list
            }
        }
    }
}