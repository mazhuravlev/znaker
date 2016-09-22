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
    public class ServerService : IGeneralComponent
    {
        private readonly Guid _componentId = new Guid("1878E9A4-4BA2-4279-B19F-11064BAED377");
        private readonly string _componentName = "ServerService-V1";


        private readonly ConcurrentQueue<ComponentContainer> _components = new ConcurrentQueue<ComponentContainer>();
        private readonly IServiceProvider _serviceProvider;
        private readonly IApplicationLifetime _appLifetime;
        private readonly ConfigurationService _configurationService;
        private readonly LoggingService _log;

        public ServerService(IServiceProvider serviceProvider, IApplicationLifetime appLifetime,
            ConfigurationService configurationService, LoggingService log)
        {
            _serviceProvider = serviceProvider;
            _appLifetime = appLifetime;
            _configurationService = configurationService;
            _log = log;
            _log.ComponetStarted(this);
        }

        public void RegisterComponent(IServerComponent component)
        {
            component.Configure(_serviceProvider);
            _components.Enqueue(new ComponentContainer(component));
            _log.Log(LogLevel.Information, this ,component, $"registred component:{component.GetName()}:{component.GetId()}");
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
                                        _log.Log(LogLevel.Information, this, e.Current.Component, "component stopped by request", null, e.Current.StoppedAt);
                                    }
                                }
                                else
                                {
                                    if (e.Current.Exception != null)
                                    {
                                        _log.Log(LogLevel.Warning, this, e.Current.Component, "component stopped with exception", e.Current.Exception, e.Current.StoppedAt);
                                        e.Current.Reset();

                                    }
                                    if (init && config.AutoStart || config.RequestStart || config.AutoRestart)
                                    {
                                        e.Current.Start();
                                        _log.Log(LogLevel.Information, this, e.Current.Component, "component started");
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
