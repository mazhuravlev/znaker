using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Grabber.Infrastructure
{
    public interface IAdvertManager
    {
        Task Run(CancellationToken cancellationToken);

        void AddGrabber(string name, IAdvertGrabber grabber);
    }
}
