using System.Threading.Tasks;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Domain.Models.BankAccounts.Enumerations;
using Yaba.Domain.Models.Users;
using Yaba.Infrastructure.DTO;
using Yaba.Tools.Validations;

namespace Yaba.Application.BankAccountServices.Impl
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBankAccountRepository _bankAccountRepository;

        public BankAccountService(
            IUserRepository userRepository,
            IBankAccountRepository bankAccountRepository)
        {
            _userRepository = userRepository;
            _bankAccountRepository = bankAccountRepository;
        }
        public async Task CreateBankAccountForUser(CreateUserBankAccountDTO dto)
        {
            var user = await _userRepository.GetById(dto.UserId);
            Validate.NotNull(user, "É necessário um usuário para criar uma conta de banco.");

            Validate.NotNullOrEmpty(dto.Agency, "É necessário fornecer uma agência.");

            Validate.NotNullOrEmpty(dto.Number, "É necessário fornecer o número da conta bancária.");

            BankCode.ValidateCode(dto.Code);

            var code = BankCode.FromValue<BankCode>(dto.Code);

            var bankAccount = new BankAccount(dto.Number, dto.Agency, code, dto.UserId);

            await _bankAccountRepository.Create(bankAccount);
        }

        public async Task<BankAccount> GetBankAccount(string agency, string number, short code)
        {
            return await _bankAccountRepository.GetBy(agency, number, code);
        }

        public async ValueTask DisposeAsync()
        {
            await _bankAccountRepository.DisposeAsync();
            await _userRepository.DisposeAsync();
        }
    }
}
