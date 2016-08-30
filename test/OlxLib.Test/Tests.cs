using OlxLib;
using Xunit;

namespace Tests
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
