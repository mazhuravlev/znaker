using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GrabberServer.Entities;
using GrabberServer.Grabbers;
using GrabberServer.Grabbers.Managers;
using Infrastructure;
using Moq;
using Xunit;

namespace SitemapGrabberManager.Test
{
    public class Tests
    {
        /// <summary>
        /// Manager must not invoke grabbers when there in no demand for jobs
        /// </summary>
        [Fact]
        public void TestWithGrabberNoDemand()
        {
            var sitemapService = new Mock<ISitemapService>();
            var adJobsService = new Mock<IAdJobsService>();
            var manager = new GrabberServer.Grabbers.Managers.SitemapGrabberManager(
                sitemapService.Object, adJobsService.Object
            )
            { CycleDelay = TimeSpan.FromMilliseconds(1) };
            var grabber = new Mock<ISitemapGrabber>();
            grabber.Setup(g => g.GrabIndex()).Returns(() => Task.FromResult(new List<SitemapEntry>()));
            manager.AddGrabber("test_grabber", grabber.Object, isEnabled: true);
            var task = manager.Run(CancellationToken.None);
            Thread.Sleep(100);
            Assert.False(task.IsFaulted);
            grabber.Verify(g => g.GrabIndex());
            grabber.Verify(g => g.HasSitemapsToGrab(It.IsAny<List<SitemapEntry>>()), Times.AtMost(0));
        }

        /// <summary>
        /// Manager must invoke grabbers when there is demand for jobs
        /// </summary>
        [Fact]
        public void TestWithGrabberWithDemand()
        {
            var sitemapService = new Mock<ISitemapService>();
            var adJobsService = new Mock<IAdJobsService>();
            var manager = new GrabberServer.Grabbers.Managers.SitemapGrabberManager(
                    sitemapService.Object, adJobsService.Object
                )
                {CycleDelay = TimeSpan.FromMilliseconds(1)};
            var grabber = new Mock<ISitemapGrabber>();
            grabber.Setup(g => g.GrabIndex()).Returns(() => Task.FromResult(new List<SitemapEntry>()));
            grabber.Setup(g => g.GetSourceType()).Returns(SourceType.Avito);
            manager.AddGrabber("test_grabber", grabber.Object, isEnabled: true);
            manager.RequestMoreJobs(new JobDemand{{SourceType.Avito, 1}});
            var task = manager.Run(CancellationToken.None);
            Thread.Sleep(100);
            Assert.False(task.IsFaulted);
            grabber.Verify(g => g.GrabIndex());
            grabber.Verify(g => g.HasSitemapsToGrab(It.IsAny<List<SitemapEntry>>()));
        }

        /// <summary>
        /// Empty manager must not fail
        /// </summary>
        [Fact]
        public void TestEmpty()
        {
            var sitemapService = new Mock<ISitemapService>();
            var adJobsService = new Mock<IAdJobsService>();
            var manager = new GrabberServer.Grabbers.Managers.SitemapGrabberManager(
                sitemapService.Object, adJobsService.Object
            )
            { CycleDelay = TimeSpan.Zero };
            var task = manager.Run(CancellationToken.None);
            Thread.Sleep(10);
            Assert.False(task.IsFaulted);
        }
    }
}
