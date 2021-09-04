using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Domain.Models.BankAccounts.Enumerations;
using Yaba.Domain.Models.Users;
using Yaba.Infrastructure.Persistence.UnitOfWork;

namespace Yaba.Tests.EntitiesCreator.UserEntity
{
    /// <summary>
    /// Create a customizable User entity with one or more BakAccounts
    /// </summary>
    public class UserBuilder
    {
        private readonly IUserRepository _userRepository;
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly UnitOfWork _uow;

        //private BankAccount _bankAccount;
        private List<BankAccount> _bankAccounts = new();
        private User _user = new("Test", "test@email.com", "123123");

        private BankCode BankCode = BankCode.GENERICBANK;

        public UserBuilder(IServiceProvider serviceProvider)
        {
            _userRepository = (IUserRepository)serviceProvider.GetService(typeof(IUserRepository));
            _bankAccountRepository = (IBankAccountRepository)serviceProvider.GetService(typeof(IBankAccountRepository));
            _uow = (UnitOfWork)serviceProvider.GetService(typeof(UnitOfWork));
        }

        public static UserBuilder CreateAUser(IServiceProvider serviceProvider)
        {
            return new UserBuilder(serviceProvider);
        }

        public UserBuilder WithName(string name)
        {
            _user.Name = name;
            return this;
        }

        public UserBuilder WithBankAccount(string number = "12345678", string agency = "12345", BankCode bankCode = null)
        {
            bankCode ??= BankCode;

            _bankAccounts.Add( new BankAccount(number, agency, bankCode, _user.Id));
            return this;
        }

        public EntitiesCreatorResponse Build()
        {
            if (_bankAccounts.Count > 0) _user.BankAccounts = _bankAccounts;
            _userRepository.Insert(_user);
            _uow.Commit();

            return new EntitiesCreatorResponse()
            {
                User = _user,
                BankAccount = _bankAccounts.FirstOrDefault(),
                BankAccounts = _bankAccounts
            };
        }
    }
}
