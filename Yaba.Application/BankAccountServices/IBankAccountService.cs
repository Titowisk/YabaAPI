using System;
using System.Threading.Tasks;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Infrastructure.DTO;

namespace Yaba.Application.BankAccountServices
{
    public interface IBankAccountService : IAsyncDisposable
    {
        Task CreateBankAccountForUser(CreateUserBankAccountDTO dto);
        Task<BankAccount> GetBankAccount(string agency, string number, short code);
    }
}
