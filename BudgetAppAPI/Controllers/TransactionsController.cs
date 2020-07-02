using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Yaba.Domain.Models.BankAccounts.Enumerations;
using Yaba.Domain.Models.Transactions;
using Yaba.Tools.Validations;

namespace Yaba.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionsController(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        [HttpPost]
        public IActionResult Create([FromBody] Transaction transaction)
        {
            try
            {
                _transactionRepository.Create(transaction);

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

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var transactions = _transactionRepository.GetAll();

                var results = from t in transactions
                              select new
                              {
                                  t.Id,
                                  t.Origin,
                                  t.Amount,
                                  t.Date,
                                  Bank = t.BankAccount != null ? BankCode.FromValue<BankCode>(t.BankAccount.Code).Name : "",
                                  Category = t.Category != null ? t.Category.ToString() : ""
                              };

                return Ok(results);
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

        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                var transaction = _transactionRepository.GetByIdWithBankAccount(id);

                Validate.NotNull(transaction, "Transaction not found");

                var result = new
                {
                    transaction.Id,
                    transaction.Origin,
                    transaction.Amount,
                    transaction.Date,
                    Bank = transaction.BankAccount != null ? BankCode.FromValue<BankCode>(transaction.BankAccount.Code).Name : "",
                    Category = transaction.Category != null ? transaction.Category.ToString() : ""
                };

                return Ok(result);
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

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            try
            {
                var transaction = _transactionRepository.GetById(id);

                Validate.NotNull(transaction, "Transaction not found");

                _transactionRepository.Delete(transaction);

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

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] Transaction newTransaction)
        {
            try
            {
                Validate.IsTrue(id == newTransaction.Id, "Parameter id must be equal to body id");

                var transaction = _transactionRepository.GetById(id);

                Validate.NotNull(transaction, "Transaction not found");

                // TODO: encapsulate Transaction creation in constructor
                transaction.SetOrigin(newTransaction.Origin);
                transaction.SetDate(newTransaction.Date);
                transaction.SetAmount(newTransaction.Amount);
                transaction.BankAccountId = newTransaction.BankAccountId;

                _transactionRepository.Update(transaction);

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
    /* NOTES:
		- Most parsers use ISO 8601 (talking about dates: 2020-01-01T17:16:40)
		- Return types
			https://docs.microsoft.com/pt-br/dotnet/api/microsoft.aspnetcore.mvc.controllerbase.accepted?view=aspnetcore-3.1
		- using .Include() normally, it brings the whole object in the response.

		- making fields nullable causes the necessity of checking values before returning
		- enum is less verbose than Enumeration when , but Enumeration is less verbose for validations
        - TODO: need logger for 500 errors
	 */
}