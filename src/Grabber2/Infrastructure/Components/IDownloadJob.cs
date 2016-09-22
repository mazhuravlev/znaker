using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Grabber2.Infrastructure.Components
{
    public interface IDownloadJob
    {
        Uri GetLink();
        void SetResult(HttpStatusCode statusCode, string response);
    }
}
