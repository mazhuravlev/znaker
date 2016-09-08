using System;
using System.Collections.Generic;
using Infrastructure;
using Microsoft.AspNetCore.Razor.Parser;

namespace GrabberServer.Grabbers.Managers
{
    public class AdJobsService
    {
        private readonly GrabberContext _grabberContext;

        public AdJobsService()
        {
        }

        public AdJobsService(GrabberContext grabberContext)
        {
            _grabberContext = grabberContext;
        }

        public void StoreAdJobs(SourceType sourceType, List<string> adIds)
        {
            throw new NotImplementedException();
        }
    }
}