using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabber.Models;
using Infrastructure;

namespace Grabber.Infrastructure
{
    public interface IAdvertService
    {
        void PushJob(AdvertJob job);
        AdvertJob GetJob(SourceType sourceType);
    }
}
