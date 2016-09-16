using System;
using System.Collections.Concurrent;
using System.Text;
using Grabber.Infrastructure.Exceptions;
using Grabber.Infrastructure.Http;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Grabber.Infrastructure.Services
{
    public interface IProxyService
    {
        Proxy GetProxy();
        void AddProxy(Proxy proxy);
    }

    public class ProxyService : IProxyService
    {
        private readonly IModel _channel;
        private readonly ConcurrentQueue<Proxy> _proxies;
        private const string QueueName = "proxy_list";

        public ProxyService()
        {
            _channel = new ConnectionFactory {HostName = "localhost"}.CreateConnection().CreateModel();
            _channel.QueueDeclare(
                queue: QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        public void AddProxy(Proxy proxy)
        {
            _proxies.Enqueue(proxy);
        }

        public Proxy GetProxy()
        {
            if (!_proxies.IsEmpty)
            {
                Proxy proxy;
                if (_proxies.TryDequeue(out proxy))
                {
                    return proxy;
                }
            }
            var proxyListEntry = _channel.BasicGet(QueueName, true);
            if (proxyListEntry == null) throw new NoResultException();
            return JsonConvert.DeserializeObject<Proxy>(Encoding.UTF8.GetString(proxyListEntry.Body));
        }
    }
}