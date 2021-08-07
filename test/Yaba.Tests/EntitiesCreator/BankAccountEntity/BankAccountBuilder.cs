using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaba.Application.BankAccountServices;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Domain.Models.BankAccounts.Enumerations;
using Yaba.Domain.Models.Users;
using Yaba.Infrastructure.Persistence.UnitOfWork;

namespace Yaba.Tests.EntitiesCreator.BankAccountEntity
{
    /// <summary>
    /// Reference: https://github.com/nicopaez/csharp_test_readability/blob/master/Domain.Tests/Support/ObjectBuilder.cs
    /// </summary>
    public class BankAccountBuilder
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IUserRepository _userRepository;
        private readonly UnitOfWork _uow;
        private readonly IBankAccountService _bankAccountService;

        public BankAccountBuilder(IServiceProvider serviceProvider)
        {
            _bankAccountRepository = (IBankAccountRepository)serviceProvider.GetService(typeof(IBankAccountRepository));
            _userRepository = (IUserRepository)serviceProvider.GetService(typeof(IUserRepository));
            _uow = (UnitOfWork)serviceProvider.GetService(typeof(UnitOfWork));


            _bankAccountService = (IBankAccountService)serviceProvider.GetService(typeof(IBankAccountService));
        }

        private BankCode BankCode = BankCode.GENERICBANK;

        public static BankAccountBuilder Create(IServiceProvider serviceProvider)
        {
            return new BankAccountBuilder(serviceProvider);
        }

        public BankAccountBuilder WithBankCode(BankCode bankCode)
        {
            this.BankCode = bankCode;

            return this;
        }

        public EntitiesCreatorResponse Build()
        {
            var user = new User("user-test", "test@email.com", "A1234567");
            _userRepository.Insert(user);
            _uow.Commit();

            var bankAccount = new BankAccount("08220842", "2514", BankCode, user.Id);
            _bankAccountRepository.Insert(bankAccount);
            _uow.Commit();

            return new EntitiesCreatorResponse { BankAccount = bankAccount, User = user };
        }
    }
}
