namespace PhoneNormalizer.CountryRules
{
    public abstract class AbstractCountryRule
    {
        public abstract string NormalizePhone(string phone);
    }
}