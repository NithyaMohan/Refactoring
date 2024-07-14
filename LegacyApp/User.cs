using System;

namespace LegacyApp
{
    public record User
    {
        public string Firstname { get; init; }
        public string Surname { get; init; }
        public DateTime DateOfBirth { get; init; }
        public string EmailAddress { get; init; }
        public bool HasCreditLimit { get; set; }
        public int CreditLimit { get; set; }
        public Client Client { get; init; }
    }
}
