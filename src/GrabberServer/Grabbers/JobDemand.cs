using System;
using System.Collections.Generic;
using Infrastructure;
using NUglify.JavaScript.Syntax;

namespace GrabberServer.Grabbers
{
    public class JobDemand : Dictionary<SourceType, int>
    {
        public static JobDemand FromList(List<KeyValuePair<SourceType, int>> list)
        {
            var jobDemand = new JobDemand();
            foreach (var keyValuePair in list)
            {
                jobDemand[keyValuePair.Key] = keyValuePair.Value;
            }
            return jobDemand;
        }
    }
}