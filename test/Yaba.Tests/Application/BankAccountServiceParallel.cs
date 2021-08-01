using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Yaba.Application.BankAccountServices;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Domain.Models.BankAccounts.Enumerations;
using Yaba.Domain.Models.Users;
using Yaba.Infrastructure.DTO;
using Yaba.Infrastructure.Persistence.UnitOfWork;

/// <summary>
/// If tests are run in Parallel, they cannot access the same file.db of sqlite (it throws an error)
/// </summary>
namespace Yaba.Tests.Application
{
    public class BankAccountServiceParallel_1
    {
        private readonly ITestOutputHelper _outputHelper;

        public BankAccountServiceParallel_1(ITestOutputHelper outputHelper)
        {
            this._outputHelper = outputHelper;
        }

        [Fact]
        public async void CreateBankAccountForUser_CreatesOneBankAccountToExistingUser()
        {
            var serviceProvider = DependencyInversion.DependencyContainer.GetServicesUsingSQLite("BankAccountServiceParallel_1");

            // given an existing user
            var userRepository = (IUserRepository)serviceProvider.GetService(typeof(IUserRepository));
            var user = new User("test", "test@email.com", "123123");
            await userRepository.Create(user);

            // when service CreateBankAccountForUser is called
            var bankAccountService = (IBankAccountService)serviceProvider.GetService(typeof(IBankAccountService));
            var dto = new CreateUserBankAccountDTO
            {
                UserId = user.Id,
                Agency = "123456",
                Code = BankCode.GENERICBANK.Value,
                Number = "12345678"
            };
            await bankAccountService.CreateBankAccountForUser(dto);

            // then the existing user should have a new BankAccount
            var bankAccountRepository = (IBankAccountRepository)serviceProvider.GetService(typeof(IBankAccountRepository));
            //var bankAccount = await bankAccountRepository.GetBy("123456", "12345678", BankCode.GENERICBANK.Value);
            var bankAccount = await bankAccountRepository.GetById(1); // this should work since each Fact has an isolated sqlite context

            _outputHelper.WriteLine("======CreateBankAccountForUser_CreatesOneBankAccountToExistingUser");
            _outputHelper.WriteLine($"Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            Assert.Equal(user.Id, bankAccount.UserId);
        }
    }

    public class BankAccountServiceParallel_2
    {
        private readonly ITestOutputHelper _outputHelper;

        public BankAccountServiceParallel_2(ITestOutputHelper outputHelper)
        {
            this._outputHelper = outputHelper;
        }

        [Fact]
        public async void DeleteBankAccount_DeleteUsersBankAccount()
        {
            var serviceProvider = DependencyInversion.DependencyContainer.GetServicesUsingSQLite("BankAccountServiceParallel_2");

            // given an existing user with one bank account
            var userRepository = (IUserRepository)serviceProvider.GetService(typeof(IUserRepository));
            var user = new User("test", "test@email.com", "123123");
            await userRepository.Create(user);

            var bankAccountService = (IBankAccountService)serviceProvider.GetService(typeof(IBankAccountService));
            var dto = new CreateUserBankAccountDTO
            {
                UserId = user.Id,
                Agency = "123456",
                Code = BankCode.GENERICBANK.Value,
                Number = "12345678"
            };
            await bankAccountService.CreateBankAccountForUser(dto);

            // when service DeleteBankAccount is called
            var bankAccountRepository = (IBankAccountRepository)serviceProvider.GetService(typeof(IBankAccountRepository));
            var bankAccount = await bankAccountRepository.GetById(1); // this should work since each Fact has an isolated sqlite context

            var deleteDTO = new DeleteUserBankAccountDTO { BankAccountId = bankAccount.Id, UserId = user.Id };
            await bankAccountService.DeleteBankAccount(deleteDTO);

            // then the user's bankaccount will be deleted
            var result = await bankAccountRepository.GetAllByUser(user.Id);

            _outputHelper.WriteLine("======DeleteBankAccount_DeleteUsersBankAccount");
            _outputHelper.WriteLine($"Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            Assert.True(!result.Any());
        }
    }

    public class BankAccountServiceParallel_3
    {
        private readonly ITestOutputHelper _outputHelper;

        public BankAccountServiceParallel_3(ITestOutputHelper outputHelper)
        {
            this._outputHelper = outputHelper;
        }

        [Fact]
        public void TestContextIsolation_1()
        {
            var serviceProvider = DependencyInversion.DependencyContainer.GetServicesUsingSQLite("BankAccountServiceParallel_3");
            var uow = (UnitOfWork)serviceProvider.GetService(typeof(UnitOfWork));

            var userRepository = (IUserRepository)serviceProvider.GetService(typeof(IUserRepository));
            var user = new User("test", "test@email.com", "123123");
            userRepository.Create(user);

            var bankAccountRepository = (IBankAccountRepository)serviceProvider.GetService(typeof(IBankAccountRepository));
            var bankAccount = new BankAccount("123", "123", BankCode.GENERICBANK, 1);
            bankAccountRepository.Insert(bankAccount);
            uow.Commit();

            _outputHelper.WriteLine("======TestContextIsolation_1");
            _outputHelper.WriteLine($"BankAccount Id: {bankAccount.Id}");
            _outputHelper.WriteLine($"Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            Assert.Equal(1, bankAccount.Id);
        }
    }

    public class BankAccountServiceParallel_4
    {
        private readonly ITestOutputHelper _outputHelper;

        public BankAccountServiceParallel_4(ITestOutputHelper outputHelper)
        {
            this._outputHelper = outputHelper;
        }

        [Fact]
        public void TestContextIsolation_2()
        {
            var serviceProvider = DependencyInversion.DependencyContainer.GetServicesUsingSQLite("BankAccountServiceParallel_4");
            var uow = (UnitOfWork)serviceProvider.GetService(typeof(UnitOfWork));

            var userRepository = (IUserRepository)serviceProvider.GetService(typeof(IUserRepository));
            var user = new User("test", "test@email.com", "123123");
            userRepository.Create(user);

            var bankAccountRepository = (IBankAccountRepository)serviceProvider.GetService(typeof(IBankAccountRepository));
            var bankAccount = new BankAccount("456", "456", BankCode.GENERICBANK, 1);
            bankAccountRepository.Insert(bankAccount);
            uow.Commit();

            _outputHelper.WriteLine("======TestContextIsolation_2");
            _outputHelper.WriteLine($"BankAccount Id: {bankAccount.Id}");
            _outputHelper.WriteLine($"Thread Id: {Thread.CurrentThread.ManagedThreadId}");

            Assert.Equal(1, bankAccount.Id);
        }
    }
}
