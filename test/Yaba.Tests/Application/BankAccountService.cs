using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
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
    public class BankAccountService
    {
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
            var bankAccount = await bankAccountRepository.GetBy("123456", "12345678", BankCode.GENERICBANK.Value);

            Assert.Equal(user.Id, bankAccount.UserId);
        }
    }
}
