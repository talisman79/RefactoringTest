using LegacyApp.Models;
using LegacyApp.Services;

namespace LegacyApp.CreditProviders
{
    public class ImportantClientCreditLimitProvider : ICreditLimitProvider
    {
        private readonly IUserCreditService _userCreditService;

        public ImportantClientCreditLimitProvider(IUserCreditService userCreditService)
        {
            _userCreditService = userCreditService;
        }

        public (bool HasCreditLimit, int CreditLimit) GetCreditLimits(User user)
        {
            var creditLimit = _userCreditService.GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth);
            
            return (true, creditLimit * 2);
        }

        public string NameRequirement { get; } = "ImportantClient";
    }
}