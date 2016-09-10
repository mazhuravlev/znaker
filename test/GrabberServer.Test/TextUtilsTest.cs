using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure;
using Xunit;

namespace GrabberServer
{
    public class TextUtilsTest
    {
        [Theory,
            InlineData("test", "test"),
            InlineData("pit bul", "pit bul"),
            InlineData("two  spaces", "two spaces"),
            InlineData("tabs\t\ttabs", "tabs tabs")]
        public void Test(string input, string expected)
        {
            Assert.Equal(expected, TextUtils.CleanSpacesAndNewlines(input));
        }
    }
}
