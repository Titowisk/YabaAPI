using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Yaba.Application.TransactionServices;
using Yaba.Infrastructure.DTO;
using Yaba.Infrastructure.DTO.Transactions;
using Yaba.Tools.Validations;

namespace Yaba.WebApi.Controllers
{
    /// <summary>
    /// TODO: https://restfulapi.net/resource-naming/
    /// https://docs.microsoft.com/en-us/azure/architecture/best-practices/api-design#what-is-rest
    /// </summary>
    [Route("api/transactions")]
    [Authorize]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(
            ITransactionService transactionService,
            ILogger<TransactionsController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        [HttpPatch]
        // transactions?origin=something&month=1&year=2021&bankAccount=12
        // transactions/{id}/similar-origin-transactions/categorize
        //[Route("{id}/similar-origin-transactions/categorize")]
        public async Task<IActionResult> CategorizeTransactions(
            [FromQuery] CategorizeTransactionsQueryDTO dtoQuery,
            [FromBody] CategorizeTransactionsBodyDTO dtoBody)
        {
            dtoQuery.UserId = GetLoggedUserId();
            await _transactionService.CategorizeTransactions(dtoQuery, dtoBody);

            return Ok();
        }

        [HttpPut]
        // transactions?origin=something
        // transactions/{id}/similar-origin-transactions/categorize
        [Route("{id}/similar-origin-transactions/categorize")]
        public async Task<IActionResult> CategorizeAllTransactionsWithSimilarOrigins(
            long id,
            [FromBody] CategorizeUserTransactionsDTO dto)
        {
            dto.UserId = GetLoggedUserId();
            await _transactionService.CategorizeAllTransactionsWithSimilarOriginsToTransactionSentByClient(dto);

            return Ok();
        }

        //[HttpPut]
        //// transactions/{id}/similar-origin-transactions/{monthId}/categorize
        //[Route("{id}/similar-origin-transactions/{month}/categorize")]
        //public async Task<IActionResult> CategorizeTransactionsWithSimilarOriginsWithinAMonth(
        //    long id,
        //    int month,
        //    [FromBody] CategorizeUserTransactionsDTO dto)
        //{
        //    dto.UserId = GetLoggedUserId();
        //    await _transactionService.CategorizeTransactionsWithSimilarOriginsWithinAMonth(dto, dto.CategoryId);

        //    return Ok();
        //}

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserTransactionDTO dto)
        {
            dto.UserId = GetLoggedUserId();
            await _transactionService.Create(dto);

            return Ok();
        }

        [HttpGet]
        [Route("bank-accounts/{id}")]
        public async Task<IActionResult> GetByDate(int id,
            [FromQuery] GetUserTransactionsByMonthDTO dto)
        {
            dto.BankAccountId = id;
            dto.UserId = GetLoggedUserId();
            var transactions = await _transactionService.GetByMonth(dto);

            return Ok(transactions);
        }

        [HttpGet]
        [Route("categories")]
        public IActionResult GetCategories()
        {
            var categories = _transactionService.GetCategories();

            return Ok(categories);
        }

        [HttpGet]
        [Route("bank-accounts/{id}/dates")]
        public async Task<IActionResult> GetTransactionDatesByUser(int id,
            [FromQuery] GetTransactionDatesDTO dto)
        {
            dto.UserId = GetLoggedUserId();
            dto.BankaccountId = id;
            var existentDates = await _transactionService.GetExistentTransactionsDatesByUser(dto);

            return Ok(existentDates);
        }

        [HttpPost]
        [Route("randomized-data")]
        public async Task<IActionResult> GenerateRandomizedDataForGenericBank([FromBody] GenerateDataDTO dto)
        {
            dto.UserId = GetLoggedUserId();
            await _transactionService.GenerateRandomizedDataForGenericBank(dto);

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