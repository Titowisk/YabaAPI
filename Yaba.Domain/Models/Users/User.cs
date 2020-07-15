using System.Collections.Generic;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Tools.Validations;

namespace Yaba.Domain.Models.Users
{
    public class User
    {
        public User(string name, string email, string password)
        {
            SetName(name);
            SetEmail(email);
            SetPassword(password);
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public IEnumerable<BankAccount> BankAccounts { get; set; }

        public void SetName(string name)
        {
            Validate.NotNullOrEmpty(name, "A name is necessary");

            Name = name;
        }

        public void SetEmail(string email)
        {
            // TODO: proper email validation
            Validate.NotNullOrEmpty(email, "An email is necessary");

            Validate.IsTrue(Name.Contains("@"), "Email is not valid");

            Email = email;
        }

        public void SetPassword(string password)
        {
            // TODO: proper password validation
            Validate.NotNullOrEmpty(password, "A password is necessary");

            Validate.IsTrue(password.Length > 7, "Password needs to be 8 characters long or greater");

            Password = password;
        }
    }
}
