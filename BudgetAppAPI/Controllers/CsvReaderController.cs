using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Yaba.Application.CsvReaderServices;
using Yaba.Domain.Models.BankAccounts.Enumerations;
using Yaba.Infrastructure.DTO;

namespace Yaba.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
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
            BankCode.ValidateCode(bankCode);

            // TODO : better way to do this? 
            var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(user?.Value))
                return Unauthorized();

            var dto = new CsvFileReaderDTO()
            {
                FilesOwnerId = int.Parse(user.Value),
                BankCode = bankCode,
                CsvFiles = csvFiles
            };

            var result = await _csvReaderService.ReadTransactionsFromFiles(dto);

            return Ok(result);
        }
    }
}
