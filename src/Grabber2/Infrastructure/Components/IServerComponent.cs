using System;
using System.Threading;

namespace Grabber2.Infrastructure.Components
{
    public interface IServerComponent : IGeneralComponent
    {
        void Configure(IServiceProvider provider);
        void Start(CancellationToken cancellationToken);
    }
}
