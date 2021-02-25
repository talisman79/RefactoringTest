using System;
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
        private readonly IUserCreditService _userCreditService;
        private readonly IUserDataAccess _userDataAccess;
        private readonly UserValidator _userValidator;

        public UserService(
            IClientRepository clientRepository, 
            IUserCreditService userCreditService, 
            IUserDataAccess userDataAccess,
            UserValidator userValidator)
        {
            _clientRepository = clientRepository;
            _userCreditService = userCreditService;
            _userDataAccess = userDataAccess;
            _userValidator = userValidator;
        }
        
        public UserService() : 
            this(new ClientRepository(),
                new UserCreditServiceClient(),
                new UserDataAccessProxy(),
                new UserValidator(new DateTimeProvider()))
        {}

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

            if (client.Name == "VeryImportantClient")
            {
                // Skip credit chek
                user.HasCreditLimit = false;
            }
            else if (client.Name == "ImportantClient")
            {
                // Do credit check and double credit limit
                user.HasCreditLimit = true;
                var creditLimit = _userCreditService.GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth);
                creditLimit = creditLimit * 2;
                user.CreditLimit = creditLimit;
            }
            else
            {
                // Do credit check
                user.HasCreditLimit = true;
                var creditLimit = _userCreditService.GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth);
                user.CreditLimit = creditLimit;
            }

            if (_userValidator.HasCreditLimitAndLimitIsLessThan500(user))
            {
                return false;
            }
            
            _userDataAccess.AddUser(user);

            return true;
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