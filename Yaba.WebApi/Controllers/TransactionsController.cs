using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Yaba.Application.TransactionServices;
using Yaba.Infrastructure.DTO;
using Yaba.Tools.Validations;

namespace Yaba.WebApi.Controllers
{
    /// <summary>
    /// TODO: https://restfulapi.net/resource-naming/
    /// https://docs.microsoft.com/en-us/azure/architecture/best-practices/api-design#what-is-rest
    /// </summary>
    [Route("api/[controller]")]
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

        [HttpPut]
        // transactions/similar-transactions/categorize
        [Route("[Action]")]
        public async Task<IActionResult> CategorizeAllTransactionsWithSimilarOrigins([FromBody] CategorizeUserTransactionsDTO dto)
        {
            dto.UserId = GetLoggedUserId();
            await _transactionService.CategorizeAllTransactionsWithSimilarOriginsToTransactionSentByClient(dto);

            return Ok();
        }

        [HttpPost]
        // transactions/
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] CreateUserTransactionDTO dto)
        {
            dto.UserId = GetLoggedUserId();
            await _transactionService.Create(dto);

            return Ok();
        }

        [HttpPost] // GET
        // transactions?month=1&year=2
        [Route("[Action]")]
        public async Task<IActionResult> GetByDate([FromBody] GetUserTransactionsByMonthDTO dto)
        {
            dto.UserId = GetLoggedUserId();
            var transactions = await _transactionService.GetByMonth(dto);

            return Ok(transactions);
        }

        [HttpGet]
        // transactions/categories
        [Route("[Action]")]
        public IActionResult GetCategories()
        {
            var categories = _transactionService.GetCategories();

            return Ok(categories);
        }

        [HttpPost]
        // transactions/dates
        [Route("[Action]")]
        public async Task<IActionResult> GetTransactionDatesByUser([FromBody] GetTransactionDatesDTO dto)
        {
            dto.UserId = GetLoggedUserId();
            var existentDates = await _transactionService.GetExistentTransactionsDatesByUser(dto);

            return Ok(existentDates);
        }

        [HttpPost]
        // transactions/randomized-data
        [Route("[Action]")]
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