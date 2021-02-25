using LegacyApp.Models;
using LegacyApp.Services;

namespace LegacyApp.CreditProviders
{
    public class DefaultCreditLimitProvider : ICreditLimitProvider
    {
        private readonly IUserCreditService _userCreditService;

        public DefaultCreditLimitProvider(IUserCreditService userCreditService)
        {
            _userCreditService = userCreditService;
        }

        public (bool HasCreditLimit, int CreditLimit) GetCreditLimits(User user)
        {
            var creditLimit = _userCreditService.GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth);

            return (true, creditLimit);
        }

        public string NameRequirement { get; } = string.Empty;
    }
}