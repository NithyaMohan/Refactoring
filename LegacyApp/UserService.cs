using System;
using System.Threading.Tasks;

namespace LegacyApp
{
    public class UserService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUserCreditService _userCreditService;
        private readonly IUserDataAccessProxy _userDataAccess;

        public UserService(IClientRepository clientRepository, IUserCreditService userCreditService, IUserDataAccessProxy userDataAccess)
        {
            _clientRepository = clientRepository;
            _userCreditService = userCreditService;
            _userDataAccess = userDataAccess;
        }

        public async Task<User> AddUser(string firstName, string surname, string email, DateTime dateOfBirth, int clientId)
        {
            
            ValidateUserInput(firstName, surname, email, dateOfBirth);

            var client = await _clientRepository.GetByIdAsync(clientId);
            if (client == null)
            {
                throw new InvalidOperationException("Client with this Id does not exist.");
            }

            var user = CreateUser(firstName, surname, email, dateOfBirth, client);

            if (user.HasCreditLimit && user.CreditLimit < 500)
            {
                throw new InvalidOperationException("insufficient credit limit");
            }

            _userDataAccess.AddUser(user);
            return user;
        }

        private void ValidateUserInput(string firstName, string surname, string email, DateTime dateOfBirth)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(surname))
            {
                throw new InvalidOperationException("user firstname / surname is required ");
            }

            if (!email.Contains('@') || (!email.Contains('.')))
            {
                throw new InvalidOperationException("user email is invalid ");
            }

            var now = DateTime.Now;
            int age = now.Year - dateOfBirth.Year;
            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day)) age--;

            if (age < 21)
            {
                throw new InvalidOperationException("user should be older than 21 years");
            }
        }

        //private bool IsValidEmail(string email)
        //{
        //    var atIndex = email.IndexOf('@');
        //    var dotIndex = email.LastIndexOf('.');
        //    return atIndex > 0 && dotIndex > atIndex + 1 && dotIndex < email.Length - 1;
        //}

        private User CreateUser(string firstName, string surname, string email, DateTime dateOfBirth, Client client)
        {
            var user = new User
            {
                Firstname = firstName,
                Surname = surname,
                EmailAddress = email,
                DateOfBirth = dateOfBirth,
                Client = client
            };

            if (client.Name == "VeryImportantClient")
            {
                user = user with { HasCreditLimit = false };
            }
            else
            {
                user.HasCreditLimit = true;
                var creditLimit = _userCreditService.GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth);

                if (client.Name == "ImportantClient")
                {
                    creditLimit *= 2;
                }

                user = user with { CreditLimit = creditLimit };
            }

            return user;
        }
    }
}
