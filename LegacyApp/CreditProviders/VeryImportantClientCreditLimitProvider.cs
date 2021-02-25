using LegacyApp.Models;

namespace LegacyApp.CreditProviders
{
    public class VeryImportantClientCreditLimitProvider : ICreditLimitProvider
    {
        public (bool HasCreditLimit, int CreditLimit) GetCreditLimits(User user)
        {
            return (false, 0);
        }

        public string NameRequirement { get; } = "VeryImportantClient";
    }
}