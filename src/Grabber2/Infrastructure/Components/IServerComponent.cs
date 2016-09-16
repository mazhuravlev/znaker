using System;
using System.Threading;

namespace Grabber2.Infrastructure.Components
{
    public interface IServerComponent
    {
        void Configure(IServiceProvider provider);
        void Start(CancellationToken cancellationToken);
        string GetName();
        Guid GetId();
    }
}
