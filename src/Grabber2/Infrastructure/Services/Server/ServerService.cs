using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grabber2.Infrastructure.Components;
using Grabber2.Infrastructure.Services.Configuration;
using Grabber2.Infrastructure.Services.Logging;
using Microsoft.AspNetCore.Hosting;

namespace Grabber2.Infrastructure.Services.Server
{
    public class ServerService
    {
        private readonly ConcurrentQueue<ComponentContainer> _components = new ConcurrentQueue<ComponentContainer>();
        private readonly IServiceProvider _serviceProvider;
        private readonly IApplicationLifetime _appLifetime;
        private readonly ConfigurationService _configurationService;
        private readonly LoggingService _loggingService;

        public ServerService(IServiceProvider serviceProvider, IApplicationLifetime appLifetime,
            ConfigurationService configurationService, LoggingService loggingService)
        {
            _serviceProvider = serviceProvider;
            _appLifetime = appLifetime;
            _configurationService = configurationService;
            _loggingService = loggingService;
        }

        public void RegisterComponent(IServerComponent component)
        {
            component.Configure(_serviceProvider);
            _components.Enqueue(new ComponentContainer(component));
            _loggingService.ComponentStatus(component, 0, "registred");
        }

        public void Start()
        {
            if (!_components.Any())
            {
                throw new Exception("Nothing to Start");
            }
            _configurationService.Refresh();

            Task.Run(() =>
            {
                try
                {
                    var init = true;
                    while (true)
                    {
                        using (var e = _components.GetEnumerator())
                        {
                            _appLifetime.ApplicationStopping.ThrowIfCancellationRequested();
                            while (e.MoveNext())
                            {
                                var config = _configurationService.GetConfiguration(e.Current.Component);
                                if (e.Current.IsRunning)
                                {
                                    if (config.RequestStop)
                                    {
                                        e.Current.Stop();
                                        _loggingService.ComponentStatus(e.Current.Component, 0, "stoped", null, e.Current.StoppedAt);
                                    }
                                }
                                else
                                {
                                    if (e.Current.Exception != null)
                                    {
                                        _loggingService.ComponentStatus(e.Current.Component, 0, "stoped", e.Current.Exception, e.Current.StoppedAt);
                                        e.Current.Reset();

                                    }
                                    if (init && config.AutoStart || config.RequestStart || config.AutoRestart)
                                    {
                                        e.Current.Start();
                                        _loggingService.ComponentStatus(e.Current.Component, 0, "started");
                                    }
                                }
                            }
                        }
                        if (init)
                        {
                            init = false;
                        }
                        Task.Delay(500, _appLifetime.ApplicationStopping).Wait(_appLifetime.ApplicationStopping);
                    }
                }
                catch (OperationCanceledException)
                {
                    foreach (var componentContainer in _components.ToArray())
                    {
                        componentContainer.Stop();
                    }
                }
            });
        }
    }
}
