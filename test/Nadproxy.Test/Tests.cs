using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using GrabberServer.Grabbers.Nadproxy;
using NuGet.Configuration;
using Xunit;

namespace Nadproxy.Test
{
    public class Tests
    {
        [Fact]
        public void TestSuccess()
        {
            var nadproxy = new SimpleNadproxy(new WebProxy("http://localhost:8888"));
            var response = nadproxy.GetAsync("http://example.com/").Result;
            Assert.IsAssignableFrom<HttpResponseMessage>(response);
            Assert.True(response.IsSuccessStatusCode);
            Assert.Contains("Example Domain", response.Content.ReadAsStringAsync().Result);
        }

        [Fact]
        public void TestFail()
        {
            var nadproxy = new SimpleNadproxy(new WebProxy("http://localhost:8888"));
            var response = nadproxy.GetAsync("http://unreachable/").Result;
            Assert.IsAssignableFrom<HttpResponseMessage>(response);
            Assert.True(!response.IsSuccessStatusCode);
        }
    }
}