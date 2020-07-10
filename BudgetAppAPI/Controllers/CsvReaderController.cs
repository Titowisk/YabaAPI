using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Yaba.Application.CsvReaderServices;
using Yaba.Domain.Models.BankAccounts.Enumerations;

namespace Yaba.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CsvReaderController : ControllerBase
    {
        private readonly ICsvReaderService _csvReaderService;
        private readonly ILogger<CsvReaderController> _logger;

        public CsvReaderController(
            ICsvReaderService csvReaderService,
            ILogger<CsvReaderController> logger)
        {
            _csvReaderService = csvReaderService;
            _logger = logger;
        }

        [Route("[Action]")]
        [HttpPost]
        public async Task<IActionResult> ReadBankStatements(IFormFileCollection csvFiles, [FromForm] short bankCode)
        {
            try
            {
                BankCode.ValidateCode(bankCode);

                var result = await _csvReaderService.ReadTransactionsFromFiles(csvFiles, bankCode);

                return Ok(result);
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, "Message: {0}", aex.Message);
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Message: {0}", ex.Message);
                return StatusCode(500);
            }

        }
    }
}
