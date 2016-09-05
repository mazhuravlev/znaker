using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PhoneNormalizer.CountryRules
{
    public class ByCountryRule : AbstractCountryRule
    {
        public override string NormalizePhone(string phone)
        {
            var regex = new Regex(@"^(375|80)(?<base>(24|25|29|33|44)\d{7})$");
            var match = regex.Match(phone);
            if (match.Success)
            {
                return "375" + match.Groups["base"].Value;
            }
            else
            {
                throw new PhoneNormalizationException();
            }
        }
    }
}