using System.Text.RegularExpressions;

namespace GrabberServer.Infrastructure.PhoneUtils.CountryRules
{
    public class UaCountryRule : AbstractCountryRule
    {
        public override string NormalizePhone(string phone)
        {
            var regex = new Regex(@"^(38)?(?<base>0(39|50|63|66|67|68|91|92|93|94|95|96|97|98|99)\d{7})$");
            var match = regex.Match(phone);
            if (match.Success)
            {
                return "38" + match.Groups["base"].Value;
            }
            else
            {
                throw new PhoneNormalizationException();
            }
        }
    }
}