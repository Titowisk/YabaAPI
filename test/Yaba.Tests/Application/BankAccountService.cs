using System.Linq;
using System.Threading;
using Xunit;
using Xunit.Abstractions;
using Yaba.Application.BankAccountServices;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Domain.Models.BankAccounts.Enumerations;
using Yaba.Domain.Models.Users;
using Yaba.Infrastructure.DTO;
using Yaba.Infrastructure.Persistence.UnitOfWork;
using Yaba.Tests.EntitiesCreator.BankAccountEntity;
using Yaba.Tests.EntitiesCreator.UserEntity;

namespace Yaba.Tests.Application
{
    /// <summary>
    /// Tests within the same class are not run in parallel
    /// Sqlite context is isolated, but access to the sqlite db file is not. So it's necessary to change the name of the sqlite db for each test collection
    /// if this tests ran in parallel, it would break
    /// </summary>
    public class BankAccountService
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly string _sqlite_db_filename;

        public BankAccountService(ITestOutputHelper outputHelper)
        {
            _sqlite_db_filename = this.GetType().Name;
            this._outputHelper = outputHelper;
        }

        [Fact]
        public async void CreateBankAccountForUser_CreatesOneBankAccountToExistingUser()
        {
            var serviceProvider = DependencyInversion.DependencyContainer.GetServicesUsingSQLite(_sqlite_db_filename);

            // given an existing user
            var response = UserBuilder.Create(serviceProvider).Build();

            // when service CreateBankAccountForUser is called
            var bankAccountService = (IBankAccountService)serviceProvider.GetService(typeof(IBankAccountService));
            var dto = new CreateUserBankAccountDTO
            {
                UserId = response.User.Id,
                Agency = "123456",
                Code = BankCode.GENERICBANK.Value,
                Number = "12345678"
            };
            await bankAccountService.CreateBankAccountForUser(dto);

            // then the existing user should have a new BankAccount
            var bankAccountRepository = (IBankAccountRepository)serviceProvider.GetService(typeof(IBankAccountRepository));
            var bankAccount = await bankAccountRepository.GetById(1); // this should work since each Fact has an isolated sqlite context

            _outputHelper.WriteLine("======CreateBankAccountForUser_CreatesOneBankAccountToExistingUser");
            _outputHelper.WriteLine($"Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            Assert.Equal(response.User.Id, bankAccount.UserId);
        }

        [Fact]
        public async void DeleteBankAccount_DeleteUsersBankAccount()
        {
            var serviceProvider = DependencyInversion.DependencyContainer.GetServicesUsingSQLite(_sqlite_db_filename);

            // given an existing user with one bank account
            var entities = BankAccountBuilder
                                    .Create(serviceProvider)
                                    .Build();

            var bankAccountService = (IBankAccountService)serviceProvider.GetService(typeof(IBankAccountService));

            // when service DeleteBankAccount is called
            var deleteDTO = new DeleteUserBankAccountDTO { BankAccountId = entities.BankAccount.Id, UserId = entities.User.Id };
            await bankAccountService.DeleteBankAccount(deleteDTO);

            // then the user's bankaccount will be deleted
            var bankAccountRepository = (IBankAccountRepository)serviceProvider.GetService(typeof(IBankAccountRepository));
            var result = await bankAccountRepository.GetAllByUser(entities.User.Id);

            _outputHelper.WriteLine("======DeleteBankAccount_DeleteUsersBankAccount");
            _outputHelper.WriteLine($"Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            Assert.True(!result.Any());
        }

        [Fact]
        public async void GetUserBankAccounts_ReturnsAllUsersBankAccounts()
        {
            var serviceProvider = DependencyInversion.DependencyContainer.GetServicesUsingSQLite(_sqlite_db_filename);

            // given a User John with two bankAccounts
            var johnResponse = UserBuilder.Create(serviceProvider)
                                        .WithBankAccount()
                                        .WithBankAccount()
                                        .WithName("John")
                                        .Build();

            //  and a BankAccount from another User called Anthony
            var anthonyResponse = UserBuilder.Create(serviceProvider)
                                                .WithName("Anthony")
                                                .Build();

            // when service GetUserBankAccounts is called for John
            var bankAccountService = (IBankAccountService)serviceProvider.GetService(typeof(IBankAccountService));
            var dto = new GetUserBankAccountsDTO() { UserId = johnResponse.User.Id };

            var result = await bankAccountService.GetUserBankAccounts(dto);

            // then it should return only two bank accounts
            Assert.Equal(2, result.Count());
            Assert.True(result.All(r => r.UserId == johnResponse.User.Id));
        }
        #region TestContextIsolation
        /// <summary>
        /// Two identical tests that verify that the created entity id is 1
        /// </summary>
        [Fact]
        public void TestContextIsolation_1()
        {
            var serviceProvider = DependencyInversion.DependencyContainer.GetServicesUsingSQLite(_sqlite_db_filename);
            var uow = (UnitOfWork)serviceProvider.GetService(typeof(UnitOfWork));

            var userRepository = (IUserRepository)serviceProvider.GetService(typeof(IUserRepository));
            var user = new User("test", "test@email.com", "123123");
            userRepository.Insert(user);
            uow.Commit();

            var bankAccountRepository = (IBankAccountRepository)serviceProvider.GetService(typeof(IBankAccountRepository));
            var bankAccount = new BankAccount("123", "123", BankCode.GENERICBANK, 1);
            bankAccountRepository.Insert(bankAccount);
            uow.Commit();

            _outputHelper.WriteLine("======TestContextIsolation_1");
            _outputHelper.WriteLine($"BankAccount Id: {bankAccount.Id}");
            _outputHelper.WriteLine($"Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            Assert.Equal(1, bankAccount.Id);
        }

        [Fact]
        public void TestContextIsolation_2()
        {
            var serviceProvider = DependencyInversion.DependencyContainer.GetServicesUsingSQLite(_sqlite_db_filename);
            var uow = (UnitOfWork)serviceProvider.GetService(typeof(UnitOfWork));

            var userRepository = (IUserRepository)serviceProvider.GetService(typeof(IUserRepository));
            var user = new User("test", "test@email.com", "123123");
            userRepository.Insert(user);
            uow.Commit();

            var bankAccountRepository = (IBankAccountRepository)serviceProvider.GetService(typeof(IBankAccountRepository));
            var bankAccount = new BankAccount("456", "456", BankCode.GENERICBANK, 1);
            bankAccountRepository.Insert(bankAccount);
            uow.Commit();

            _outputHelper.WriteLine("======TestContextIsolation_2");
            _outputHelper.WriteLine($"BankAccount Id: {bankAccount.Id}");
            _outputHelper.WriteLine($"Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            Assert.Equal(1, bankAccount.Id);
        }
        #endregion
    }
}
