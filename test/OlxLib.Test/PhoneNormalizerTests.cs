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
        //http://simcard.kz/

        [Fact]
        public void TestPhoneNormalizer()
        {
            Assert.Throws<ArgumentException>(() => PhoneNormalizer.Normalize(""));
            Assert.Throws<ArgumentException>(() => PhoneNormalizer.Normalize(null));
            Assert.Throws<ArgumentException>(() => PhoneNormalizer.Normalize("  "));
            Assert.Throws<FormatException>(() => PhoneNormalizer.Normalize("pit bul"));
            Assert.Throws<FormatException>(() => PhoneNormalizer.Normalize("12345"));
            Assert.Throws<FormatException>(() => PhoneNormalizer.Normalize("12345 12345 12345"));
            Assert.Throws<FormatException>(() => PhoneNormalizer.Normalize("11118543065"));
            Assert.Throws<FormatException>(() => PhoneNormalizer.Normalize("8 067 856 4569")); // old ua format


            var actual = "79788543065"; //ru
            Assert.Equal(PhoneNormalizer.Normalize("+79788543065"), actual);
            Assert.Equal(PhoneNormalizer.Normalize("+7(978)8543065"), actual);
            Assert.Equal(PhoneNormalizer.Normalize("+7 978) 854 30 65"), actual);
            Assert.Equal(PhoneNormalizer.Normalize("89788543065"), actual);
            Assert.Equal(PhoneNormalizer.Normalize("suka:89788543065"), actual);
            Assert.Equal(PhoneNormalizer.Normalize("978-854-30-65"), actual);

            actual = "380678564569"; // ua
            Assert.Equal(PhoneNormalizer.Normalize("+380678564569"), actual);
            Assert.Equal(PhoneNormalizer.Normalize("+38 067 856 4569"), actual);
            Assert.Equal(PhoneNormalizer.Normalize(" 067 856 4569"), actual);
            Assert.Equal(PhoneNormalizer.Normalize("0678564569"), actual);
            Assert.Equal(PhoneNormalizer.Normalize("06785-64569"), actual);
            Assert.Equal(PhoneNormalizer.Normalize("pidor:0678564569"), actual);

            actual = "375295830468"; //by
            Assert.Equal(PhoneNormalizer.Normalize("+375295830468"), actual);
            Assert.Equal(PhoneNormalizer.Normalize("+3752 95830 468"), actual);
            Assert.Equal(PhoneNormalizer.Normalize("80295830468"), actual);

            actual = "77027007077"; //kz
            Assert.Equal(PhoneNormalizer.Normalize("+77027007077"), actual);
            Assert.Equal(PhoneNormalizer.Normalize("+77 02 7007 077"), actual);
            Assert.Equal(PhoneNormalizer.Normalize("87027007077"), actual);
            Assert.Equal(PhoneNormalizer.Normalize("7027007077"), actual);

            actual = "998994055804"; //uz
            Assert.Equal(PhoneNormalizer.Normalize("+998994055804"), actual);
            Assert.Equal(PhoneNormalizer.Normalize("99 89 94055 804"), actual);
        }
    }
}
