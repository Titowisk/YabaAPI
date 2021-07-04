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
        [Route("[Action]")]
        public async Task<IActionResult> CategorizeAllTransactionsWithSimilarOrigins([FromBody] CategorizeUserTransactionsDTO dto)
        {
            dto.UserId = GetLoggedUserId();
            await _transactionService.CategorizeAllTransactionsWithSimilarOrigins(dto);

            return Ok();
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] CreateUserTransactionDTO dto)
        {
            dto.UserId = GetLoggedUserId();
            await _transactionService.Create(dto);

            return Ok();
        }

        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> GetByDate([FromBody] GetUserTransactionsByMonthDTO dto)
        {
            dto.UserId = GetLoggedUserId();
            var transactions = await _transactionService.GetByMonth(dto);

            return Ok(transactions);
        }

        [HttpGet]
        [Route("[Action]")]
        public IActionResult GetCategories()
        {
            var categories = _transactionService.GetCategories();

            return Ok(categories);
        }

        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> GetTransactionDatesByUser([FromBody] GetTransactionDatesDTO dto)
        {
            dto.UserId = GetLoggedUserId();
            var existentDates = await _transactionService.GetExistentTransactionsDatesByUser(dto);

            return Ok(existentDates);
        }

        [HttpPost]
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
    /* NOTES:
		- Most parsers use ISO 8601 (talking about dates: 2020-01-01T17:16:40)
		- Return types
			https://docs.microsoft.com/pt-br/dotnet/api/microsoft.aspnetcore.mvc.controllerbase.accepted?view=aspnetcore-3.1
		- using .Include() normally, it brings the whole object in the response.

		- making fields nullable causes the necessity of checking values before returning
		- enum is less verbose than Enumeration when , but Enumeration is less verbose for validations
	 */
}