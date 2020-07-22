using System.Threading.Tasks;
using Yaba.Infrastructure.DTO;

namespace Yaba.Application.BankAccountServices
{
    public interface IBankAccountService
    {
        Task CreateBankAccountForUser(CreateUserBankAccountDTO dto);
    }
}
