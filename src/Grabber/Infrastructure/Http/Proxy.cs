using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Grabber.Infrastructure.Http
{
    public class Proxy : IWebProxy
    {
        public string Host;
        public int Port;
        public Uri GetProxy(Uri destination)
        {
            return new Uri($"http://{Host}:{Port}");
        }

        public bool IsBypassed(Uri host)
        {
            return false;
        }

        public ICredentials Credentials { get; set; }
    }
}
