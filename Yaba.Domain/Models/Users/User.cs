using System.Collections.Generic;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Tools.Validations;

namespace Yaba.Domain.Models.Users
{
    public class User
    {
        public User(string name, string email, string password)
        {
            Name = name;
            Email = email;
            Password = password;
        }

        public int Id { get; private set; }
        public string Name { get; set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public ICollection<BankAccount> BankAccounts { get; set; }
    }
}
