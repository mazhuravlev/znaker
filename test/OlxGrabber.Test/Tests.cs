using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using GrabberServer.Entities;
using GrabberServer.Grabbers.Olx;
using Xunit;

namespace OlxGrabber.Test
{
    public class Tests
    {
        [Fact]
        public void TestSitemapGrabber()
        {
            var olxConfig = new OlxConfig(
                OlxType.Ua,
                "http://olx.ua/sitemap.xml",
                "https://ssl.olx.ua/i2/obyavlenie/?json=1&id={0}&version=2.3.2",
                "https://ssl.olx.ua/i2/ajax/ad/getcontact/?type=phone&json=1&id={0}&version=2.3.2"
            );
            var sitemapGrabber = new OlxSitemapGrabber(olxConfig, new HttpClient());
            var sitemapIndex = sitemapGrabber.GrabIndex().Result;
            Assert.NotEmpty(sitemapIndex);
            Assert.IsType(typeof(SitemapEntry), sitemapIndex.First());
            Assert.False(sitemapGrabber.HasSitemapsToGrab(new List<SitemapEntry>()));

            Assert.True(sitemapGrabber.HasSitemapsToGrab(sitemapIndex));

            var sitemapResult = sitemapGrabber.GrabNextSitemap(sitemapIndex).Result;
            Assert.NotEmpty(sitemapResult.AdIds);
            Assert.Contains(sitemapResult.SitemapEntry, sitemapIndex);
        }
    }
}
