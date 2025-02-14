using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Yaba.Application.CsvReaderServices;
using Yaba.Infrastructure.DTO;

namespace Yaba.WebApi.Controllers
{
    [Route("api/csv-management")]
    [Authorize]
    public class CsvReaderController : BaseYabaController
    {
        private readonly ICsvReaderService _csvReaderService;
        private readonly ILogger<CsvReaderController> _logger;

        public CsvReaderController(
            ICsvReaderService csvReaderService,
            ILogger<CsvReaderController> logger)
        {
            this._csvReaderService = csvReaderService;
            this._logger = logger;
        }

        [Route("statements")]
        [HttpPost]
        public async Task<IActionResult> ReadBankStatements(IFormFileCollection csvFiles, [FromForm] int bankAccountId)
        {
            var userId = base.GetLoggedUserId();

            var dto = new CsvFileReaderDTO()
            {
                FilesOwnerId = userId,
                BankAccountId = bankAccountId,
                CsvFiles = csvFiles
            };

            var result = await this._csvReaderService.ReadTransactionsFromFiles(dto);

            return this.Ok(result);
        }
    }
}
