using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PhoneNormalizer.CountryRules
{
    public class RuCountryRule : AbstractCountryRule
    {
        public override string NormalizePhone(string phone)
        {
            var regex = new Regex(@"^(8|7)?(?<base>9(00|01|02|03|04|05|06|08|09|10|11|12|13|14|15|16|17|18|19|20|21|22|23|24|25|26|27|28|29|30|31|32|33|34|36|37|38|39|50|51|52|53|58|60|61|62|63|64|65|66|67|68|69|77|78|80|81|82|83|84|85|86|87|88|89|91|92|93|94|95|96|99)\d{7})$");
            var match = regex.Match(phone);
            if (match.Success)
            {
                return "7" + match.Groups["base"].Value;
            }
            else
            {
                throw new PhoneNormalizationException();
            }
        }
    }
}