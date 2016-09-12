using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabber.Models;
using Infrastructure;
using RabbitMQ.Client;

namespace Grabber.Managers
{
    public interface IAdJobsService
    {
        void PushAdJob(AdGrabJob job);
        AdGrabJob GetJob(SourceType sourceType);
    }

    public class AdJobsService : IAdJobsService
    {
        private readonly IModel _channel;

        public AdJobsService()
        {
            var factory = new ConnectionFactory() {HostName = "localhost"};
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
        }

        public void PushAdJob(AdGrabJob job)
        {
            _channel.QueueDeclare(queue: QueueName(job.SourceType),
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            _channel.BasicPublish(exchange: "",
                routingKey: QueueName(job.SourceType),
                basicProperties: null,
                body: Encoding.UTF8.GetBytes(job.AdId));
        }

        public AdGrabJob GetJob(SourceType sourceType)
        {
            _channel.QueueDeclare(queue: QueueName(sourceType),
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            var result = _channel.BasicGet(QueueName(sourceType), true);
            return result == null ? null : new AdGrabJob
            {
                SourceType = sourceType,
                AdId = System.Text.Encoding.UTF8.GetString(result.Body)
            };
        }

        private static string QueueName(SourceType sourceType)
        {
            return "ad_jobs_" + sourceType;

        }
    }
}