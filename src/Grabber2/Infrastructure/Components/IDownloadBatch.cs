using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Grabber2.Infrastructure.Components
{
    public interface IDownloadBatch
    {
        IDownloadJob GetJob();
        bool IsComplete();
    }
}
