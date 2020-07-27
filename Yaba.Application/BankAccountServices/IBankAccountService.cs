using System;
using System.Threading.Tasks;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Infrastructure.DTO;

namespace Yaba.Application.BankAccountServices
{
    public interface IBankAccountService : IDisposable
    {
        Task CreateBankAccountForUser(CreateUserBankAccountDTO dto);
        Task UpdateBankAccount(UpdateUserBankAccountDTO dto);
        Task DeleteBankAccount(DeleteUserBankAccountDTO dto);
        Task<BankAccount> GetBankAccountBy(GetUserBankAccountDTO dto);
        Task<BankAccountResponseDTO> GetBankAccountById(GetUserBankAccountDTO dto);
    }
}
