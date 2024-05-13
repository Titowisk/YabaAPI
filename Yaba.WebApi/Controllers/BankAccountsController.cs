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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BankAccountsController : BaseYabaController
    {
        private readonly IBankAccountService _bankAccountService;

        public BankAccountsController(
            ILogger<BankAccountsController> logger,
            IBankAccountService bankAccountService)
        {
            _bankAccountService = bankAccountService;
        }

        [HttpGet]
        [Route("[Action]")]
        public async Task<ActionResult<IEnumerable<BankAccountsResponseDTO>>> GetBankAccountsByUser()
        {
            var dto = new GetUserBankAccountsDTO()
            {
                UserId = GetLoggedUserId()
            };

            var bankAccounts = await _bankAccountService.GetUserBankAccounts(dto);

            return Ok(bankAccounts);
        }

        // GET: api/BankAccounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BankAccountResponseDTO>> GetBankAccount(int id)
        {
            var dto = new GetUserBankAccountDTO()
            {
                BankAccountId = id,
                UserId = GetLoggedUserId()
            };

            var bankAccount = await _bankAccountService.GetBankAccountById(dto);

            return bankAccount;
        }

        // PUT: api/BankAccounts/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBankAccount(int id, UpdateUserBankAccountDTO dto)
        {
            dto.UserId = GetLoggedUserId();
            dto.BankAccountId = id;
            await _bankAccountService.UpdateBankAccount(dto);

            return NoContent();
        }

        // POST: api/BankAccounts
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<BankAccount>> Create(CreateUserBankAccountDTO dto)
        {
            dto.UserId = GetLoggedUserId();

            await _bankAccountService.CreateBankAccountForUser(dto);

            // TODO: create common success handler

            var response = new
            {
                Success = true,
                Data = "Conta bancária criada com sucesso"
            };

            return Ok(response);
        }

        // DELETE: api/BankAccounts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<BankAccount>> DeleteBankAccount(int id)
        {
            var dto = new DeleteUserBankAccountDTO()
            {
                UserId = GetLoggedUserId(),
                BankAccountId = id
            };

            await _bankAccountService.DeleteBankAccount(dto);

            return Ok();
        }
    }
}
