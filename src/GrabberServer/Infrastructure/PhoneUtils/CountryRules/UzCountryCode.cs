namespace GrabberServer.Infrastructure.PhoneUtils.CountryRules
{
    public class UzCountryRule : AbstractCountryRule
    {
        public override string NormalizePhone(string phone)
        {
            throw new PhoneNormalizationException();
        }
    }
}