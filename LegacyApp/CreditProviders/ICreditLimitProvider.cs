using LegacyApp.Models;

namespace LegacyApp.CreditProviders
{
    public interface ICreditLimitProvider
    {
        (bool HasCreditLimit, int CreditLimit) GetCreditLimits(User user);
        
        public string NameRequirement { get; }
    }
}