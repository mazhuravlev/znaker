using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OlxLib.Utils;
using OlxLib.Workers;
using Xunit;

namespace OlxLib.Test
{
    public class PhoneNormalizerTests
    {

        [Fact]
        public void TestNonPhones()
        {
            Assert.Throws<ArgumentException>(() => PhoneNormalizer.Normalize(""));
            Assert.Throws<ArgumentException>(() => PhoneNormalizer.Normalize(null));
            Assert.Throws<ArgumentException>(() => PhoneNormalizer.Normalize("  "));
            Assert.Throws<FormatException>(() => PhoneNormalizer.Normalize("pit bul"));
            Assert.Throws<FormatException>(() => PhoneNormalizer.Normalize("12345"));
            Assert.Throws<FormatException>(() => PhoneNormalizer.Normalize("12345 12345 12345"));
            Assert.Throws<FormatException>(() => PhoneNormalizer.Normalize("11118543065"));
        }


        [Fact]
        public void TestRuMobile()
        {
            var actual = "79788543065";
            Assert.Equal(PhoneNormalizer.Normalize("+79788543065"), actual);
            Assert.Equal(PhoneNormalizer.Normalize("+7(978)8543065"), actual);
            Assert.Equal(PhoneNormalizer.Normalize("+7 978) 854 30 65"), actual);
            Assert.Equal(PhoneNormalizer.Normalize("89788543065"), actual);
            Assert.Equal(PhoneNormalizer.Normalize("suka:89788543065"), actual);
            Assert.Equal(PhoneNormalizer.Normalize("978-854-30-65"), actual);
        }

        [Fact]
        public void TestUaMobile()
        {
            var actual = "380678564569";
            Assert.Equal(PhoneNormalizer.Normalize("+380678564569"), actual);
            Assert.Equal(PhoneNormalizer.Normalize("+38 067 856 4569"), actual);
            Assert.Equal(PhoneNormalizer.Normalize("8 067 856 4569"), actual);
            Assert.Equal(PhoneNormalizer.Normalize("0678564569"), actual);
            Assert.Equal(PhoneNormalizer.Normalize("06785-64569"), actual);
            Assert.Equal(PhoneNormalizer.Normalize("pidor:0678564569"), actual);
        }

    }
}
