using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GrabberServer.Entities;
using GrabberServer.Grabbers;
using GrabberServer.Grabbers.Managers;
using GrabberServer.Infrastructure.PhoneUtils;
using GrabberServer.Infrastructure.PhoneUtils.CountryRules;
using Infrastructure;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace Tests
{
    public class AdDownloadManagerTest
    {
        private readonly CancellationTokenSource TokenSource = new CancellationTokenSource();

        //[Fact]
        public void Test()
        {
            var adJobsService = new Mock<IAdJobsService>();
            adJobsService.Setup(ajs => ajs.GetJobs(It.IsAny<JobDemand>())).Returns(() => new JobDemandResult());
            var manager = new AdDownloadManager(adJobsService.Object);
            var task = manager.Run(TokenSource.Token);
            while (task.Status != TaskStatus.Running)
            {
                Thread.Sleep(10);
            }
            Thread.Sleep(10);
            Assert.Equal(task.Status, TaskStatus.Running);
            adJobsService.Verify(ajs => ajs.GetJobs(It.IsAny<JobDemand>()));
            TokenSource.Cancel();
        }

        //[Fact]
        public void TestSitemapsRequest()
        {
            var adJobsService = new Mock<IAdJobsService>();
            adJobsService.Setup(ajs => ajs.GetJobs(It.IsAny<JobDemand>())).Returns(() => new JobDemandResult());
            var sitemapGrabberManager = new Mock<ISitemapGrabberManager>();
            var grabber = new Mock<IAdGrabber>();
            grabber.Setup(g => g.GetSourceType()).Returns(SourceType.Avito);
            var manager = new AdDownloadManager(adJobsService.Object, sitemapGrabberManager.Object);
            manager.AddGrabber("test_gabber", grabber.Object);
            var task = manager.Run(TokenSource.Token);
            while (task.Status != TaskStatus.Running)
            {
                Thread.Sleep(10);
            }
            Thread.Sleep(10);
            Assert.Equal(task.Status, TaskStatus.Running);
            adJobsService.Verify(ajs => ajs.GetJobs(It.IsAny<JobDemand>()));
            sitemapGrabberManager.Verify(g => g.RequestMoreJobs(It.IsAny<JobDemand>()));
            TokenSource.Cancel();
        }
    }
}