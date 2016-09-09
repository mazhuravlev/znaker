using System;
using System.Collections.Generic;
using GrabberServer.Entities;
using GrabberServer.Grabbers;
using GrabberServer.Infrastructure.PhoneUtils;
using GrabberServer.Infrastructure.PhoneUtils.CountryRules;
using Infrastructure;
using Xunit;
using Assert = Xunit.Assert;

namespace Tests
{
    public class Tests
    {
        [Fact]
        public void TestNotSatisfied()
        {
            var demand = new JobDemand
            {
                {SourceType.Avito, 10}
            };
            var demandResult = new JobDemandResult
            {
                {SourceType.Avito, new List<AdDownloadJob>()}
            };
            Assert.False(demandResult.DoesSatisfyDemand(demand));
        }

        [Fact]
        public void TestSatisfied()
        {
            var demand = new JobDemand
            {
                {SourceType.Avito, 1}
            };
            var demandResult = new JobDemandResult
            {
                {SourceType.Avito, new List<AdDownloadJob> {new AdDownloadJob()}}
            };
            Assert.True(demandResult.DoesSatisfyDemand(demand));
        }
    }
}