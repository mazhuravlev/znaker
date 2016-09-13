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
            var factory = new ConnectionFactory {HostName = "localhost"};
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
        }

        public void PushJob(AdvertJob job)
        {
            _channel.QueueDeclare(
                queue: QueueName(job.SourceType),
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            _channel.BasicPublish(
                exchange: "",
                routingKey: QueueName(job.SourceType),
                basicProperties: null,
                body: Encoding.UTF8.GetBytes(job.Id)
            );
        }

        public AdvertJob GetJob(SourceType sourceType)
        {
            _channel.QueueDeclare(
                queue: QueueName(sourceType),
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var result = _channel.BasicGet(QueueName(sourceType), true);

            return result == null ? null : new AdvertJob
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