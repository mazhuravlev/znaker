using System;
using System.Collections.Generic;
using GrabberServer.Infrastructure.PhoneUtils;
using GrabberServer.Infrastructure.PhoneUtils.CountryRules;
using Xunit;
using Assert = Xunit.Assert;

namespace GrabberServer.Test
{
    public class PhoneUtilsTest
    {
        [Fact]
        public void TestInvalidPhonesError()
        {
            var normalizer = new PhoneNormalizer(new List<AbstractCountryRule>
            {
                new RuCountryRule(),
                new UaCountryRule(),
                new ByCountryRule(),
                //new KzCountryRule(),
                //new UzCountryRule()
            });

            Assert.Throws<ArgumentException>(() => normalizer.Normalize(""));
            Assert.Throws<ArgumentException>(() => normalizer.Normalize(null));
            Assert.Throws<ArgumentException>(() => normalizer.Normalize("  "));
            Assert.Throws<PhoneNormalizationException>(() => normalizer.Normalize("pit bul"));
            Assert.Throws<PhoneNormalizationException>(() => normalizer.Normalize("12345"));
            Assert.Throws<PhoneNormalizationException>(() => normalizer.Normalize("12345 12345 12345"));
            Assert.Throws<PhoneNormalizationException>(() => normalizer.Normalize("11118543065"));
            Assert.Throws<PhoneNormalizationException>(() => normalizer.Normalize("8 067 856 4569")); // old ua format*
        }

        [Fact]
        public void TestRu()
        {
            const string actual = "79788543065";
            var normalizer = new PhoneNormalizer(new RuCountryRule());
            Assert.Equal(normalizer.Normalize("+79788543065"), actual);
            Assert.Equal(normalizer.Normalize("+7(978)8543065"), actual);
            Assert.Equal(normalizer.Normalize("+7 978) 854 30 65"), actual);
            Assert.Equal(normalizer.Normalize("89788543065"), actual);
            Assert.Equal(normalizer.Normalize("suka:89788543065"), actual);
            Assert.Equal(normalizer.Normalize("978-854-30-65"), actual);
        }

        [Fact]
        public void TestUa()
        {
            const string actual = "380678564569";
            var normalizer = new PhoneNormalizer(new UaCountryRule());
            Assert.Equal(normalizer.Normalize("+380678564569"), actual);
            Assert.Equal(normalizer.Normalize("+38 067 856 4569"), actual);
            Assert.Equal(normalizer.Normalize(" 067 856 4569"), actual);
            Assert.Equal(normalizer.Normalize("0678564569"), actual);
            Assert.Equal(normalizer.Normalize("06785-64569"), actual);
            Assert.Equal(normalizer.Normalize("pidor:0678564569"), actual);
        }

        [Fact]
        public void TestBy()
        {
            const string actual = "375295830468";
            var normalizer = new PhoneNormalizer(new ByCountryRule());
            Assert.Equal(normalizer.Normalize("+375295830468"), actual);
            Assert.Equal(normalizer.Normalize("+3752 95830 468"), actual);
            Assert.Equal(normalizer.Normalize("80295830468"), actual);
        }

        //[Fact]
        public void TestKz()
        {
            const string actual = "77027007077";
            var normalizer = new PhoneNormalizer(new KzCountryRule());
            Assert.Equal(normalizer.Normalize("+77027007077"), actual);
            Assert.Equal(normalizer.Normalize("+77 02 7007 077"), actual);
            Assert.Equal(normalizer.Normalize("87027007077"), actual);
            Assert.Equal(normalizer.Normalize("7027007077"), actual);
        }

        //[Fact]
        public void TestUz()
        {
            const string actual = "998994055804";
            var normalizer = new PhoneNormalizer(new UzCountryRule());
            Assert.Equal(normalizer.Normalize("+998994055804"), actual);
            Assert.Equal(normalizer.Normalize("99 89 94055 804"), actual);
        }
    }
}
