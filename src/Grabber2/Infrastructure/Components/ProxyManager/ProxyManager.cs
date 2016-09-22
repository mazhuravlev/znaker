using System;
using System.Threading;
using System.Threading.Tasks;
using Grabber2.Infrastructure.Services.Logging;
using Grabber2.Infrastructure.Services.Server;
using Microsoft.Extensions.DependencyInjection;

namespace Grabber2.Infrastructure.Components.ProxyManager
{
    public class ProxyManager : IServerComponent
    {
        private readonly Guid _componentId = new Guid("CB8E34B4-D13A-4D64-AC16-FB329D86334B");
        private readonly string _componentName = "ProxyManager-v1";

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
                _log.Log(LogLevel.Information , this, "ping " + t++);
                Task.Delay(2500, cancellationToken).Wait(cancellationToken);
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
