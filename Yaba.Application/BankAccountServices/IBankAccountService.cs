using System;
using System.Threading.Tasks;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Infrastructure.DTO;

namespace Yaba.Application.BankAccountServices
{
    public interface IBankAccountService : IAsyncDisposable
    {
        Task CreateBankAccountForUser(CreateUserBankAccountDTO dto);
        Task UpdateBankAccount(UpdateUserBankAccountDTO dto);
        Task<BankAccount> GetBankAccountBy(GetUserBankAccountDTO dto);
        Task<BankAccountResponseDTO> GetBankAccountById(GetUserBankAccountDTO dto);
    }
}
