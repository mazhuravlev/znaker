using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Grabber2.Infrastructure.Components;
using Grabber2.Infrastructure.Services.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Grabber2.Infrastructure.Services.Logging
{
    public class LoggingService
    {
        private readonly ILogger _logger;
        private readonly ConcurrentQueue<LoggingModel> _logs = new ConcurrentQueue<LoggingModel>();
        private readonly IApplicationLifetime _appLifetime;

        public LoggingService(ILogger<LoggingService> logger, IApplicationLifetime appLifetime)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            Save();
        }

        public void Save()
        {
            Task.Run(() =>
            {
                while (!_appLifetime.ApplicationStopping.IsCancellationRequested)
                {

                    //save _logs to db

                    Task.Delay(2000, _appLifetime.ApplicationStopping).Wait(_appLifetime.ApplicationStopping);
                }
            });
        }


        public void Log(IServerComponent component, int level, string message, Exception ex = null, DateTime? createAt = null)
        {
            message += ex?.Message;
            _logs.Enqueue(new LoggingModel
            {
                CreatedAt = createAt ?? DateTime.Now,
                ComponentId = component.GetId(),
                Level = level,
                Message = message,
                LogType = 1
            });
            Console(component, message, ex);
        }

        public void ComponentStatus(IServerComponent component, int level, string message, Exception ex = null, DateTime? createAt = null)
        {
            message += ex?.Message;
            _logs.Enqueue(new LoggingModel
            {
                CreatedAt = createAt ?? DateTime.Now,
                ComponentId = component.GetId(),
                Level = level,
                Message = message,
                LogType = 1
            });
            Console(component, message, ex);
        }

        private void Console(IServerComponent component, string message, Exception ex = null)
        {
            if (ex != null)
            {
                _logger.LogError(component.GetName() + ": " + message);
            }
            else
            {
                _logger.LogInformation(component.GetName() + ": " + message);
            }
            
        }
    }
}
