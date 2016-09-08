using System.Threading;
using GrabberServer.Grabbers.Managers;
using Moq;
using Xunit;

namespace SitemapGrabberManager.Test
{
    public class Tests
    {
        [Fact]
        public void TestEmpty()
        {
            var sitemapService = new Mock<SitemapService>();
            var manager = new GrabberServer.Grabbers.Managers.SitemapGrabberManager(sitemapService.Object);
            var c = new CancellationTokenSource();
            manager.Run(c.Token);
            Thread.Sleep(manager.CycleDelay * 3);
            c.Cancel();
        }
    }
}
