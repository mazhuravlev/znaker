using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GrabberServer.Entities;
using GrabberServer.Grabbers;
using GrabberServer.Grabbers.Managers;
using GrabberServer.Grabbers.Olx;
using Moq;
using Xunit;

namespace SitemapGrabberManager.Test
{
    public class Tests
    {
        [Fact]
        public void TestEmpty()
        {
            var sitemapService = new Mock<ISitemapService>();
            var adJobsService = new Mock<AdJobsService>();
            var manager = new GrabberServer.Grabbers.Managers.SitemapGrabberManager(
                sitemapService.Object, adJobsService.Object
            ) {CycleDelay = 0};
            var task = manager.Run(CancellationToken.None);
            Thread.Sleep(100);
            Assert.Equal(TaskStatus.Running, task.Status);
        }

        [Fact]
        public void TestWithWithGrabber()
        {
            var sitemapService = new Mock<ISitemapService>();
            var adJobsService = new Mock<AdJobsService>();
            var manager = new GrabberServer.Grabbers.Managers.SitemapGrabberManager(
                sitemapService.Object, adJobsService.Object
            ) {CycleDelay = 1};
            var grabber = new Mock<ISitemapGrabber>();
            grabber.Verify(g => g.GrabIndex());
            grabber.Verify(g => g.HasSitemapsToGrab(It.IsAny<List<SitemapEntry>>()));
            manager.AddGrabber("olx_ua", grabber.Object, isEnabled: true);
            var task = manager.Run(CancellationToken.None);
            Thread.Sleep(10);
            Assert.Equal(TaskStatus.Running, task.Status);
        }
    }
}
