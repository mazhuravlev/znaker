using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OlxLib;
using Infrastructure;
using PhoneNormalizer.CountryRules;

namespace PhoneNormalizer
{
    public class PhoneNormalizer
    {
        private static readonly Regex CleanRegex = new Regex(@"\D");
        private readonly List<AbstractCountryRule> _rules;

        public PhoneNormalizer(List<AbstractCountryRule> rules)
        {
            _rules = rules;
        }

        public PhoneNormalizer(AbstractCountryRule rule)
        {
            _rules = new List<AbstractCountryRule> {rule};
        }

        public string Normalize(string phone)
        {
            if (!phone.HasText())
            {
                throw new ArgumentException("Phone number is null or empty");
            }
            var cleanPhone = CleanRegex.Replace(phone, "");
            foreach (var rule in _rules)
            {
                try
                {
                    return rule.NormalizePhone(cleanPhone);
                }
                catch (PhoneNormalizationException)
                {
                    continue;
                }
            }
            throw new PhoneNormalizationException();
        }
    }
}