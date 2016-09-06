namespace GrabberServer.Infrastructure.PhoneUtils.CountryRules
{
    public abstract class AbstractCountryRule
    {
        public abstract string NormalizePhone(string phone);
    }
}