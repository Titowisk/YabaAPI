﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yaba.Application.BankAccountServices;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Domain.Models.BankAccounts.Enumerations;
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

        // GET: api/BankAccounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BankAccount>>> GetBankAccounts()
        {
            try
            {
                var bankAccounts = await _bankAccountRepository.GetAll();

                Validate.IsTrue(bankAccounts.Count() > 0, "No bank accounts found.");

                return Ok(bankAccounts);
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, "Message: {0}", aex.Message); 
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Message: {0}", ex.Message);
                return StatusCode(500);
            }

        }

        // GET: api/BankAccounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BankAccount>> GetBankAccount(int id)
        {
            try
            {
                var bankAccount = await _bankAccountRepository.GetById(id);

                Validate.NotNull(bankAccount, "Bank account not found");

                return bankAccount;
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, "Message: {0}", aex.Message);
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Message: {0}", ex.Message);
                return StatusCode(500);
            }
        }

        // PUT: api/BankAccounts/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBankAccount(int id, BankAccount bankAccount)
        {
            try
            {
                Validate.IsTrue(id == bankAccount.Id, "Parameter id must be equal to body id");

                Validate.IsTrue(await _bankAccountRepository.Exists(id), $"Bank account {bankAccount.Number} not found");

                BankCode.ValidateCode(bankAccount.Code);

                await _bankAccountRepository.Update(bankAccount);
                return NoContent();
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, "Message: {0}", aex.Message);
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Message: {0}", ex.Message);
                return StatusCode(500);
            }
        }

        // POST: api/BankAccounts
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<BankAccount>> Create(CreateUserBankAccountDTO dto)
        {
            try
            {
                await _bankAccountService.CreateBankAccountForUser(dto);

                return Ok("Conta bancária criada com sucesso");
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, "Message: {0}", aex.Message);
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Message: {0}", ex.Message);
                return StatusCode(500);
            }
        }

        // DELETE: api/BankAccounts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<BankAccount>> DeleteBankAccount(int id)
        {
            try
            {
                var bankAccount = _bankAccountRepository.GetById(id);

                Validate.NotNull(bankAccount, "Bank account not found.");

                await _bankAccountRepository.Delete(id);

                return Ok();
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, "Message: {0}", aex.Message);
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Message: {0}", ex.Message);
                return StatusCode(500);
            }
        }
    }
    /*NOTES:
     -file generated using Visual Studio Controller Scaffold
     - deals with DbUpdateConcurrencyException
     - uses EntityState.Modified fpr update*/
}
