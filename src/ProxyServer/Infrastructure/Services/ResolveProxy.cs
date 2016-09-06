using System;
using NuGet.Configuration;
using ProxyServer.Infrastructure.Middleware;

namespace ProxyServer.Infrastructure.Services
{
    public class ResolveProxy
    {
        public WebProxy GetEndPointProxy()
        {
            return new WebProxy(new Uri("http://94.20.21.38:8888"));
        }
    }
}
