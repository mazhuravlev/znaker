using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Grabber.Infrastructure.Services
{
    public class NoResultException : Exception
    {
    }

    public class Proxy
    {
        public string Host;
        public int Port;
    }

    public interface IProxyService
    {
        Proxy GetProxy();
    }

    public class ProxyService
    {
        private readonly IModel _channel;
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

        public Proxy GetProxy()
        {
            var proxyListEntry = _channel.BasicGet(QueueName, true);
            if (proxyListEntry == null) throw new NoResultException();
            return JsonConvert.DeserializeObject<Proxy>(Encoding.UTF8.GetString(proxyListEntry.Body));
        }
    }