using System;
using System.Threading;

namespace Grabber2.Infrastructure
{
    public interface IGeneralComponent
    {
        string GetName();
        Guid GetId();
    }
}
