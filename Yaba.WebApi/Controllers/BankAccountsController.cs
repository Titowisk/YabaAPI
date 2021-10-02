using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yaba.Application.BankAccountServices;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Infrastructure.DTO;

namespace Yaba.WebApi.Controllers
{
    /// <summary>
    /// TODO: change names
    /// </summary>
    [Route("api/[controller]")]
    [Authorize]
    public class BankAccountsController : BaseYabaController
    {
        private readonly ILogger<BankAccountsController> _logger;
        private readonly IBankAccountService _bankAccountService;

        public BankAccountsController(
            ILogger<BankAccountsController> logger,
            IBankAccountService bankAccountService)
        {
            this._logger = logger;
            this._bankAccountService = bankAccountService;
        }

        [HttpGet("logged-user")]
        // bank-accounts/
        public async Task<ActionResult<IEnumerable<BankAccountsResponseDTO>>> GetByLoggedUser()
        {
            var bankAccountsDto = new GetUserBankAccountsDTO()
            {
                UserId = base.GetLoggedUserId()
            };

            var bankAccounts = await this._bankAccountService.GetUserBankAccounts(bankAccountsDto);

            return this.Ok(bankAccounts);
        }

        [HttpGet("{id}")]
        // bank-accounts/{id}
        public async Task<ActionResult<BankAccountResponseDTO>> Get(int id)
        {
            var dto = new GetUserBankAccountDTO()
            {
                BankAccountId = id,
                UserId = base.GetLoggedUserId()
            };

            var bankAccount = await this._bankAccountService.GetBankAccountById(dto);

            return bankAccount;
        }

        // PUT: api/BankAccounts/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        // bank-accounts/{id}
        public async Task<IActionResult> Update(int id, UpdateUserBankAccountDTO dto)
        {
            dto.UserId = base.GetLoggedUserId();
            dto.BankAccountId = id;
            await this._bankAccountService.UpdateBankAccount(dto);

            return this.NoContent();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        // bank-accounts/
        public async Task<ActionResult<BankAccount>> Create(CreateUserBankAccountDTO dto)
        {
            dto.UserId = base.GetLoggedUserId();

            await this._bankAccountService.CreateBankAccountForUser(dto);

            // TODO: create common success handler

            var response = new
            {
                Success = true,
                Data = "Conta bancária criada com sucesso"
            };

            return this.Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<BankAccount>> Delete(int id)
        {
            var dto = new DeleteUserBankAccountDTO()
            {
                UserId = base.GetLoggedUserId(),
                BankAccountId = id
            };

            await this._bankAccountService.DeleteBankAccount(dto);

            return this.NoContent();
        }
    }
}
