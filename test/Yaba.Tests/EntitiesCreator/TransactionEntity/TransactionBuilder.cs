using Bogus; //TODO: this package exists in a referenced project (Application), what happens if a uninstall Bogus from project Application? Is it ok to use packages like this?
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Domain.Models.BankAccounts.Enumerations;
using Yaba.Domain.Models.Transactions;
using Yaba.Domain.Models.Transactions.Enumerations;
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
        private readonly ITransactionRepository _transactionRepository;
        private readonly UnitOfWork _uow;

        private List<Transaction> _transactions = new();
        private User _user = new("Test", "test@email.com", "123123");
        private BankAccount _bankAccount;

        public TransactionBuilder(IServiceProvider serviceProvider)
        {
            _userRepository = (IUserRepository)serviceProvider.GetService(typeof(IUserRepository));
            _bankAccountRepository = (IBankAccountRepository)serviceProvider.GetService(typeof(IBankAccountRepository));
            _transactionRepository = (ITransactionRepository)serviceProvider.GetService(typeof(ITransactionRepository));
            _uow = (UnitOfWork)serviceProvider.GetService(typeof(UnitOfWork));

            _userRepository.Insert(_user);
            _uow.Commit();

            _bankAccount = new BankAccount("08220842", "2514", BankCode.GENERICBANK, _user.Id);
            _bankAccountRepository.Insert(_bankAccount);
            _uow.Commit();
        }

        public static TransactionBuilder Create(IServiceProvider serviceProvider)
        {
            return new TransactionBuilder(serviceProvider);
        }

        public TransactionBuilder AddSingleTransaction(string origin, DateTime date, decimal amount, Category? category = null)
        {
            var transaction = new Transaction(origin, date, amount, _bankAccount.Id);
            if (category.HasValue) transaction.Category = category;
            _transactions.Add(transaction);
            return this;
        }

        public TransactionBuilder AddManyExpenseTransactions(int quantity, int year = default, int month = default, Category category = Category.Savings, string origin = default)
        {
            
            var fakeExpenseTransactions = new Faker<Transaction>()
                .RuleFor(t => t.BankAccountId, _bankAccount.Id)
                .RuleFor(t => t.Date, 
                    f => new DateTime(
                        year == 0 ? f.Random.Int(2015, 2019) : year,
                        month == 0 ? f.Random.Int(1, 12) : month, 
                        f.Random.Int(1, 27)))
                .RuleFor(t => t.Category, category)
                .RuleFor(t => t.Amount, f => f.Finance.Amount() * -1)
                .RuleFor(t => t.Origin, f => string.IsNullOrEmpty(origin) ? f.Company.CompanyName() : origin)
                .RuleFor(t => t.Metadata, f => $"GenericBank_{Guid.NewGuid()}")
                .Generate(quantity);

            _transactions.AddRange(fakeExpenseTransactions);
            return this;
        }

        public EntitiesCreatorResponse Build()
        {
            //_userRepository.Insert(_user);
            //_uow.Commit();

            //_bankAccount = new BankAccount("08220842", "2514", BankCode.GENERICBANK, _user.Id);
            _transactionRepository.InsertRange(_transactions);
            //_bankAccountRepository.Insert(_bankAccount);
            _uow.Commit();

            return new EntitiesCreatorResponse()
            {
                Transactions = _bankAccount.Transactions,
                BankAccount = _bankAccount,
                User = _user
            };
        }
    }
}
