using System.Collections.Generic;
using Infrastructure;
using NUglify.JavaScript.Syntax;

namespace GrabberServer.Grabbers
{
    public class JobDemand : List<KeyValuePair<SourceType, int>>
    {
        public static JobDemand FromList(List<KeyValuePair<SourceType, int>> list)
        {
            var jobDemand = new JobDemand();
            jobDemand.AddRange(list);
            return jobDemand;
        }
    }
}