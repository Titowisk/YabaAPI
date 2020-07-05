using Microsoft.AspNetCore.Http;
using Yaba.Infrastructure.DTO;

namespace Yaba.Application.BankStatementReaders
{
    public interface IBankEstatementReader
    {
        StandardBankStatementDTO ProcessBankInformation(IFormFile csvFile);

        // TODO: validate csv file as a valid bankstatement based on an existent bankCode
    }
}
