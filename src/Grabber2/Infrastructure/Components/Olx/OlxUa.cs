using System;
using System.Threading;
using System.Threading.Tasks;
using Grabber2.Infrastructure.Services.Logging;
using Grabber2.Infrastructure.Services.Server;
using Microsoft.Extensions.DependencyInjection;

namespace Grabber2.Infrastructure.Components.Olx
{
    public class OlxUa : IServerComponent
    {
        private readonly Guid _componentId = new Guid("037049F7-41A3-47F0-ADDE-F99F1D55AFD2");
        private readonly string _componentName = "OlxUaGrabber-v1";


        private LoggingService _log;
        public void Configure(IServiceProvider provider)
        {
            _log = provider.GetService<LoggingService>();
            //throw new NotImplementedException();
        }

        public void Start(CancellationToken cancellationToken)
        {
            _log.ComponetStarted(this);
            var t = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                if (t++ % 100 == 0)
                {
                    _log.Log(LogLevel.Information, this, "fast" + t);
                }
                Task.Delay(50, cancellationToken).Wait(cancellationToken);
            }
            //throw new NotImplementedException();
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
