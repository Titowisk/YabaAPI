using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
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

        public CsvReaderController(ICsvReaderService csvReaderService)
        {
            _csvReaderService = csvReaderService;

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
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500);
            }

        }
    }
}
