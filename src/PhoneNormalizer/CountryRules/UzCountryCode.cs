using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PhoneNormalizer.CountryRules
{
    public class UzCountryRule : AbstractCountryRule
    {
        public override string NormalizePhone(string phone)
        {
            throw new PhoneNormalizationException();
        }
    }
}