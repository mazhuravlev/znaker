using System;
using System.Collections.Generic;
using System.Linq;
using GrabberServer.Entities;
using Infrastructure;

namespace GrabberServer.Grabbers
{
    public class JobDemandResult : Dictionary<SourceType, List<AdDownloadJob>>
    {
        public bool DoesSatisfyDemand(JobDemand jobDemand)
        {
            return this.Count != 0 &&
                   jobDemand.All(entry => ContainsKey(entry.Key) && this[entry.Key].Count >= entry.Value);
        }
    }
}