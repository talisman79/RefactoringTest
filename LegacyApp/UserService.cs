using System;
using LegacyApp.CreditProviders;
using LegacyApp.DataAccess;
using LegacyApp.Models;
using LegacyApp.Repositories;
using LegacyApp.Services;
using LegacyApp.Validators;

namespace LegacyApp
{
    public class UserService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUserDataAccess _userDataAccess;
        private readonly UserValidator _userValidator;
        private readonly CreditLimitProviderFactory _creditLimitProviderFactory;

        public UserService(
            IClientRepository clientRepository, 
            IUserDataAccess userDataAccess,
            UserValidator userValidator,
            CreditLimitProviderFactory creditLimitProviderFactory)
        {
            _clientRepository = clientRepository;
            _userDataAccess = userDataAccess;
            _userValidator = userValidator;
            _creditLimitProviderFactory = creditLimitProviderFactory;
        }
        
        public UserService() : 
            this(new ClientRepository(),
                new UserDataAccessProxy(),
                new UserValidator(new DateTimeProvider()),
                new CreditLimitProviderFactory(new UserCreditServiceClient()))
        {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="firname"></param>
        /// <param name="surname"></param>
        /// <param name="email"></param>
        /// <param name="dateOfBirth"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public bool AddUser(string firname, string surname, string email, DateTime dateOfBirth, int clientId)
        {
            if (!UserProvidedDataIsValid(firname, surname, email, dateOfBirth))
            {
                return false;
            }

            var client = _clientRepository.GetById(clientId);

            var user = new User
            {
                Client = client,
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                Firstname = firname,
                Surname = surname
            };

            ApplyCreditLimits(client, user);

            if (_userValidator.HasCreditLimitAndLimitIsLessThan500(user))
            {
                return false;
            }
            
            _userDataAccess.AddUser(user);

            return true;
        }

        private void ApplyCreditLimits(Client client, User user)
        {
            var provider = _creditLimitProviderFactory.GetProviderByClientName(client.Name);
            var (hasCreditLimit, creditLimit) = provider.GetCreditLimits(user);
            user.HasCreditLimit = hasCreditLimit;
            user.CreditLimit = creditLimit;
        }

        private bool UserProvidedDataIsValid(string firname, string surname, string email, DateTime dateOfBirth)
        {
            if (!_userValidator.HasValidFullName(firname, surname))
            {
                return false;
            }

            if (!_userValidator.HasValidEmail(email))
            {
                return false;
            }

            if (!_userValidator.IsUserIsAtLeast21YearsOld(dateOfBirth))
            {
                return false;
            }

            return true;
        }
    }
}