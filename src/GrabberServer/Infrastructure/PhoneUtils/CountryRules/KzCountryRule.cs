namespace GrabberServer.Infrastructure.PhoneUtils.CountryRules
{
    public class KzCountryRule : AbstractCountryRule
    {
        public override string NormalizePhone(string phone)
        {
            throw new PhoneNormalizationException();
        }
    }
}