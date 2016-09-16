using System;
using System.Threading;
using System.Threading.Tasks;
using Grabber2.Infrastructure.Services.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Grabber2.Infrastructure.Components.AvitoRu
{
    public class AviroRu : IServerComponent
    {
        private readonly Guid _componentId = new Guid("3B5C3E28-DC1B-4033-9B69-64F82DF77CFC");
        private LoggingService _log;
        public void Configure(IServiceProvider provider)
        {
            _log = provider.GetService<LoggingService>();
            //throw new NotImplementedException();
        }

        public void Start(CancellationToken cancellationToken)
        {
            var t = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                _log.Log(this, 1, "ping" + t);


                if (t++ > 3)
                {
                    throw new Exception("Sheeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeet");
                }

                Task.Delay(1500, cancellationToken).Wait(cancellationToken);
            }
            //throw new NotImplementedException();
        }

        public string GetName()
        {
            return "AvitoRu-v1";
        }

        public Guid GetId()
        {
            return _componentId;
        }
    }
}
