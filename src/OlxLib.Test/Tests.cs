using OlxLib.Utils;
using Xunit;

namespace OlxLib.Test
{
    public class Tests
    {
        [Fact]
        public void TestRightResult() 
        {
            Assert.Equal(IdUtils.DecryptOlxId("fzaOY"), 230028120);
        }

        [Fact]
        public void TestWrongResult()
        {
            Assert.NotEqual(IdUtils.DecryptOlxId("fzaOY"), 1);
        }
    }
}
