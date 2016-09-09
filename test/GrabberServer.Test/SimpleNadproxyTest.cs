using System;
using System.Net.Http;
using NuGet.Configuration;
using Xunit;

namespace GrabberServer.Test
{
    public class SimpleNadproxyTest
    {
        private const string GoodProxyUrl = "http://localhost:8888";
        private const string BadProxyUrl = "http://unreachable";
        private const string GoodUrl = "http://example.com";
        private const string BadUrl = "http://unreachable";

        [Fact]
        public void TestSuccess()
        {
            var nadproxy = new GrabberServer.Grabbers.Nadproxy.SimpleNadproxy(new WebProxy(GoodProxyUrl));
            var response = nadproxy.GetAsync(GoodUrl).Result;
            Assert.IsAssignableFrom<HttpResponseMessage>(response);
            Assert.True(response.IsSuccessStatusCode);
            Assert.Contains("Example Domain", response.Content.ReadAsStringAsync().Result);
        }

        [Fact]
        public void TestBadUrl()
        {
            var nadproxy = new GrabberServer.Grabbers.Nadproxy.SimpleNadproxy(new WebProxy(GoodProxyUrl));
            var response = nadproxy.GetAsync(BadUrl).Result;
            Assert.IsAssignableFrom<HttpResponseMessage>(response);
            Assert.True(!response.IsSuccessStatusCode);
        }

        [Fact]
        public void TestBadProxyFail()
        {
            var nadproxy = new GrabberServer.Grabbers.Nadproxy.SimpleNadproxy(new WebProxy(BadProxyUrl));
            Assert.ThrowsAny<Exception>(() => nadproxy.GetAsync(GoodUrl).Result);
        }
    }
}