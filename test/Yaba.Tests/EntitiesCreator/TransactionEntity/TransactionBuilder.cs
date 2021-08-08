using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Domain.Models.BankAccounts.Enumerations;
using Yaba.Domain.Models.Transactions;
using Yaba.Domain.Models.Users;
using Yaba.Infrastructure.Persistence.UnitOfWork;

namespace Yaba.Tests.EntitiesCreator.TransactionEntity
{
    /// <summary>
    /// Creates customizable transaction entities for a default bank account
    /// </summary>
    public class TransactionBuilder
    {
        private readonly IUserRepository _userRepository;
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly UnitOfWork _uow;

        private List<Transaction> _transactions = new();
        private User _user = new("Test", "test@email.com", "123123");
        private BankAccount _bankAccount;

        public TransactionBuilder(IServiceProvider serviceProvider)
        {
            _userRepository = (IUserRepository)serviceProvider.GetService(typeof(IUserRepository));
            _bankAccountRepository = (IBankAccountRepository)serviceProvider.GetService(typeof(IBankAccountRepository));
            _uow = (UnitOfWork)serviceProvider.GetService(typeof(UnitOfWork));
        }

        public static TransactionBuilder Create(IServiceProvider serviceProvider)
        {
            return new TransactionBuilder(serviceProvider);
        }

        public TransactionBuilder AddTransaction(string origin, DateTime date, decimal amount)
        {
            _transactions.Add(new Transaction(origin, date, amount, _bankAccount.Id));
            return this;
        }

        public EntitiesCreatorResponse Build()
        {
            _userRepository.Insert(_user);
            _uow.Commit();

            _bankAccount = new BankAccount("08220842", "2514", BankCode.GENERICBANK, _user.Id);
            _bankAccountRepository.Insert(_bankAccount);
            
            return new EntitiesCreatorResponse()
            {
                BankAccount = _bankAccount,
                User = _user
            };
        }
    }
}
