﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Yaba.Application.BankAccountServices;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Infrastructure.DTO;
using Yaba.Tools.Validations;

namespace Yaba.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BankAccountsController : ControllerBase
    {
        private readonly ILogger<BankAccountsController> _logger;
        private readonly IBankAccountService _bankAccountService;
        private readonly IBankAccountRepository _bankAccountRepository;

        public BankAccountsController(
            ILogger<BankAccountsController> logger,
            IBankAccountRepository bankAccountRepository,
            IBankAccountService bankAccountService)
        {
            _logger = logger;
            _bankAccountService = bankAccountService;
            _bankAccountRepository = bankAccountRepository;
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

        #region Priv Methods
        private int GetLoggedUserId()
        {
            // TODO : better way to do this? 
            var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            Validate.IsTrue(!string.IsNullOrEmpty(user.Value), "Acesso negado");

            return int.Parse(user.Value);
        }
        #endregion
    }
}
