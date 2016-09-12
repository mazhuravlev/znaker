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
    }

    public class AdJobsService : IAdJobsService
    {
        private readonly IModel _channel;

        public AdJobsService()
        {
            var factory = new ConnectionFactory() {HostName = "localhost"};
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(queue: "ad_jobs",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        public void PushAdJob(AdGrabJob job)
        {
            _channel.BasicPublish(exchange: "",
                routingKey: "ad_jobs",
                basicProperties: null,
                body: Encoding.UTF8.GetBytes(job.AdId));
        }
    }
}