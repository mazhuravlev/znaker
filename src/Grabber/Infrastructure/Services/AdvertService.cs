using System.Text;
using Grabber.Models;
using Infrastructure;
using RabbitMQ.Client;

namespace Grabber.Infrastructure.Services
{
    public class AdvertService : IAdvertService
    {
        private readonly IModel _channel;

        public AdvertService()
        {
            _channel = new ConnectionFactory {HostName = "localhost"}.CreateConnection().CreateModel();
            _channel.QueueDeclare(
                queue: QueueName(SourceType.OlxUa),
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        public void PushJob(AdvertJob job)
        {
            _channel.BasicPublish(
                exchange: "",
                routingKey: QueueName(job.SourceType),
                basicProperties: null,
                body: Encoding.UTF8.GetBytes(job.Id)
            );
        }

        public AdvertJob GetJob(SourceType sourceType)
        {
            var result = _channel.BasicGet(QueueName(sourceType), true);
            return result == null
                ? null
                : new AdvertJob
                {
                    SourceType = sourceType,
                    Id = Encoding.UTF8.GetString(result.Body)
                };
        }

        private static string QueueName(SourceType sourceType)
        {
            return "ad_jobs_" + sourceType;
        }
    }
}