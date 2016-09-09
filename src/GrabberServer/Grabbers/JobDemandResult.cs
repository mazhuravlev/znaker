using System;
using System.Collections.Generic;
using System.Linq;
using GrabberServer.Entities;
using Infrastructure;

namespace GrabberServer.Grabbers
{
    public class JobDemandResult : List<KeyValuePair<SourceType, List<AdDownloadJob>>>
    {
        public bool DoesSatisfyDemand(JobDemand jobDemand)
        {
            if (this.Count == 0)
            {
                return false;
            }
            foreach (var entry in jobDemand)
            {
                KeyValuePair<SourceType, List<AdDownloadJob>> myRecord;
                try
                {
                    myRecord = this.First(k => k.Key == entry.Key);
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
                if (myRecord.Value.Count < entry.Value)
                {
                    return false;
                }
            }
            return true;
        }
    }
}