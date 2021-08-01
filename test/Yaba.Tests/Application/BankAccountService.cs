using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Yaba.Application.BankAccountServices;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Domain.Models.BankAccounts.Enumerations;
using Yaba.Domain.Models.Users;
using Yaba.Infrastructure.DTO;
using Yaba.Infrastructure.Persistence.Context;
using Yaba.Infrastructure.Persistence.Repositories;
using Yaba.Tests.DependencyInversion;

namespace Yaba.Tests.Application
{
    // TODO: Create another FACT and check the behave of database context: is it really totally separeted?
    // Or are they sharing the same context? (see AddScoped DataContext)
    // Check how Xunit make parallel tests
    public class BankAccountService
    {
        private readonly ITestOutputHelper _outputHelper;

        public BankAccountService(ITestOutputHelper outputHelper)
        {
            this._outputHelper = outputHelper;
        }

        [Fact]
        public async void CreateBankAccountForUser_CreatesOneBankAccountToExistingUser()
        {
            var serviceProvider = DependencyInversion.DependencyContainer.GetServicesUsingSQLite();

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

            Assert.Equal(user.Id, bankAccount.UserId);
        }

        [Fact]
        public async void DeleteBankAccount_DeleteUsersBankAccount()
        {
            var serviceProvider = DependencyInversion.DependencyContainer.GetServicesUsingSQLite();

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

            Assert.True(!result.Any());
        }
    }
}
