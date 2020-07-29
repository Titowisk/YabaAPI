using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Yaba.Application.TransactionServices;
using Yaba.Domain.Models.BankAccounts.Enumerations;
using Yaba.Domain.Models.Transactions;
using Yaba.Infrastructure.DTO;
using Yaba.Tools.Validations;

namespace Yaba.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(
            ITransactionService transactionService,
            ITransactionRepository transactionRepository,
            ILogger<TransactionsController> logger)
        {
            _transactionService = transactionService;
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Create([FromBody] Transaction transaction)
        {
            try
            {
                _transactionRepository.Insert(transaction);

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

        [HttpGet]
        [Route("[Action]")]
        public async Task<IActionResult> GetByDate([FromBody] GetUserTransactionsByMonthDTO dto)
        {
            try
            {
                dto.UserId = GetLoggedUserId();
                var transactions = await _transactionService.GetByMonth(dto);

                return Ok(transactions);
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

        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                var transaction = _transactionRepository.GetByIdWithBankAccount(id);

                Validate.NotNull(transaction, "Transaction not found");

                //var result = new
                //{
                //    transaction.Id,
                //    transaction.Origin,
                //    transaction.Amount,
                //    transaction.Date,
                //    Bank = transaction.BankAccount != null ? BankCode.FromValue<BankCode>(transaction.BankAccount.Code).Name : "",
                //    Category = transaction.Category != null ? transaction.Category.ToString() : ""
                //};

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var dto = new DeleteUserTransactionDTO()
                {
                    UserId = GetLoggedUserId(),
                    TransactionId = id
                };

                //await _transactionService.Delete(dto);

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


        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] TransactionDTO newTransaction)
        {
            try
            {
                Validate.IsTrue(id == newTransaction.Id, "Parameter id must be equal to body id");

                var transaction = _transactionRepository.GetById(id);

                Validate.NotNull(transaction, "Transaction not found");

                //transaction.SetOrigin(newTransaction.Origin);
                //transaction.SetDate(newTransaction.Date);
                //transaction.SetAmount(newTransaction.Amount);
                //transaction.BankAccountId = newTransaction.BankAccountId;

                //_transactionRepository.Update(transaction);

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
    /* NOTES:
		- Most parsers use ISO 8601 (talking about dates: 2020-01-01T17:16:40)
		- Return types
			https://docs.microsoft.com/pt-br/dotnet/api/microsoft.aspnetcore.mvc.controllerbase.accepted?view=aspnetcore-3.1
		- using .Include() normally, it brings the whole object in the response.

		- making fields nullable causes the necessity of checking values before returning
		- enum is less verbose than Enumeration when , but Enumeration is less verbose for validations
	 */
}