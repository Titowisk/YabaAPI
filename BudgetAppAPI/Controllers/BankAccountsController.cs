using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yaba.Tools.Validations;
using YabaAPI.Models;
using YabaAPI.Repositories;
using YabaAPI.Repositories.Contracts;

namespace YabaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankAccountsController : ControllerBase
    {
        private readonly IBankAccountRepository _bankAccountRepository;

        public BankAccountsController(IBankAccountRepository bankAccountRepository)
        {
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
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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

                Validate.NotNull(bankAccount);

                return bankAccount;
            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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

                Validate.IsTrue(await _bankAccountRepository.Exists(id));

                // TODO: bankAccount.Code must be validated, or else, invalid short values can be insert on the table
                BankCode.FromValue<BankCode>(bankAccount.Code);
           
                await _bankAccountRepository.Update(bankAccount);
                return NoContent();
            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }

        // POST: api/BankAccounts
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<BankAccount>> PostBankAccount(BankAccount bankAccount)
        {
            try
            {
                await _bankAccountRepository.Create(bankAccount);

                return CreatedAtAction("GetBankAccount", new { id = bankAccount.Id }, bankAccount);
            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }
    }
    /*NOTES:
     -file generated using Visual Studio Controller Scaffold
     - deals with DbUpdateConcurrencyException
     - uses EntityState.Modified fpr update*/
}
