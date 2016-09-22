using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Grabber2.Infrastructure.Components;
using Grabber2.Infrastructure.Services.Logging;

namespace Grabber2.Infrastructure.Services.Network
{
    public class NetworkService : IGeneralComponent
    {
        private readonly Guid _componentId = new Guid("C2C35AA0-C02B-4A0C-9F29-F01AD6A98ADE");
        private readonly string _componentName = "NetworkService-V1";

        private readonly ConcurrentDictionary<IServerComponent, ConcurrentQueue<IDownloadBatch>> _processing = new ConcurrentDictionary<IServerComponent, ConcurrentQueue<IDownloadBatch>>();
        private readonly ConcurrentDictionary<IServerComponent, ConcurrentQueue<IDownloadBatch>> _complete = new ConcurrentDictionary<IServerComponent, ConcurrentQueue<IDownloadBatch>>();

        private readonly LoggingService _log;

        public NetworkService(LoggingService log)
        {
            _log = log;
            _log.ComponetStarted(this);
        }

        private ConcurrentQueue<IDownloadBatch> GetProcessingQueue(IServerComponent component)
        {
            return _processing.GetOrAdd(component, new ConcurrentQueue<IDownloadBatch>());
        }
        private ConcurrentQueue<IDownloadBatch> GetCompleteQueue(IServerComponent component)
        {
            return _complete.GetOrAdd(component, new ConcurrentQueue<IDownloadBatch>());
        }

        public bool AddJob(IServerComponent component, IDownloadBatch batch, int maxCapacity)
        {
            var quenue = GetProcessingQueue(component);
            if (quenue.Count > maxCapacity)
            {
                return false;
            }
            _log.Log(LogLevel.Information, this, component, "add downloadJob");
            quenue.Enqueue(batch);
            return true;
        }
        public bool GetCompleted(IServerComponent component, out IDownloadBatch batch)
        {
            return GetCompleteQueue(component).TryDequeue(out batch);
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
