using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Grabber2.Infrastructure.Components;
using Grabber2.Infrastructure.Services.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;

namespace Grabber2.Infrastructure.Services.Logging
{
    public class LoggingService : IGeneralComponent
    {
        private readonly Guid _componentId = new Guid("35D41D02-B4AE-45DB-A955-1B35950A6612");
        private readonly string _componentName = "LoggingService-V1";

        private readonly Guid _sessionId;
        private readonly ILoggerFactory _logger;
        private readonly ConcurrentDictionary<string, ILogger> _loggers = new ConcurrentDictionary<string, ILogger>();

        private readonly ConcurrentQueue<LoggingModel> _logs = new ConcurrentQueue<LoggingModel>();
        private readonly IApplicationLifetime _appLifetime;
        private readonly IHostingEnvironment _env;

        public LoggingService(ILoggerFactory logger, IApplicationLifetime appLifetime, IHostingEnvironment env)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _env = env;
            _sessionId = Guid.NewGuid();;
            Save();
            ComponetStarted(this);
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

        public void ComponetStarted(IGeneralComponent component)
        {
            Log(LogLevel.Information, component, $"Started ({component.GetName()}:{component.GetId()})");
        }


        public void Log(LogLevel lvl, IGeneralComponent component, string message, Exception ex = null, DateTime? createAt = null)
        {
            Log(lvl, component, null, message, ex, createAt);    
        }

        public void Log(LogLevel level, IGeneralComponent component, IGeneralComponent relatedComponent, string message, Exception ex = null , DateTime? createAt = null)
        {
            _logs.Enqueue(new LoggingModel
            {
                SessionId = _sessionId,
                ComponentId = component.GetId(),
                RelatedComponentId = relatedComponent?.GetId(),
                CreatedAt = createAt ?? DateTime.Now,
                Level = level,
                Message = message,
                Exception = ex?.ToString()
            });

            var loggerName = component.GetName();
            if (relatedComponent != null)
            {
                loggerName += $": ({relatedComponent.GetName()})";
            }
            var l = _loggers.GetOrAdd(loggerName, _logger.CreateLogger(loggerName));
            var msg = $"{createAt ?? DateTime.Now:s} {message}";
            switch (level)
            {
                case LogLevel.Trace:       l.LogTrace(0, ex, msg); break;
                case LogLevel.Debug:       l.LogDebug(0, ex, msg); break;
                case LogLevel.Information: l.LogInformation(0, ex, msg); break;
                case LogLevel.Warning:     l.LogWarning(0, ex, msg); break;
                case LogLevel.Error:       l.LogError(0, ex, msg); break;
                case LogLevel.Critical:    l.LogCritical(0, ex, msg); break;
            }
        }

        public string GetName()
        {
            return _componentName;
        }

        public Guid GetId()
        {
            return _componentId;
        }
    }
}
