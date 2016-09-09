using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GrabberServer.Entities;
using GrabberServer.Grabbers;
using GrabberServer.Grabbers.Managers;
using Moq;
using Xunit;

namespace SitemapGrabberManager.Test
{
    public class Tests
    {
        [Fact]
        public void TestWithGrabber()
        {
            var sitemapService = new Mock<ISitemapService>();
            var adJobsService = new Mock<AdJobsService>();
            var manager = new GrabberServer.Grabbers.Managers.SitemapGrabberManager(
                sitemapService.Object, adJobsService.Object
            )
            { CycleDelay = TimeSpan.FromMilliseconds(1) };
            var grabber = new Mock<ISitemapGrabber>();
            grabber.Setup(g => g.GrabIndex()).Returns(() => Task.FromResult(new List<SitemapEntry>()));
            manager.AddGrabber("test_grabber", grabber.Object, isEnabled: true);
            var task = manager.Run(CancellationToken.None);
            Thread.Sleep(10);
            Assert.Equal(TaskStatus.Running, task.Status);
            grabber.Verify(g => g.GrabIndex());
            grabber.Verify(g => g.HasSitemapsToGrab(It.IsAny<List<SitemapEntry>>()));
        }

        [Fact]
        public void TestEmpty()
        {
            var sitemapService = new Mock<ISitemapService>();
            var adJobsService = new Mock<AdJobsService>();
            var manager = new GrabberServer.Grabbers.Managers.SitemapGrabberManager(
                sitemapService.Object, adJobsService.Object
            )
            { CycleDelay = TimeSpan.Zero };
            var task = manager.Run(CancellationToken.None);
            Thread.Sleep(10);
            Assert.Equal(TaskStatus.Running, task.Status);
        }
    }
}
