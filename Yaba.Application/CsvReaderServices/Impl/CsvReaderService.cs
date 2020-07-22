using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yaba.Application.BankStatementReaders;
using Yaba.Application.BankStatementReaders.ReaderResolver;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Domain.Models.BankAccounts.Enumerations;
using Yaba.Domain.Models.Transactions;
using Yaba.Infrastructure.DTO;
using Yaba.Tools.Validations;

namespace Yaba.Application.CsvReaderServices.Impl
{
    public class CsvReaderService : ICsvReaderService
    {
        private readonly ILogger<CsvReaderService> _logger;
        private readonly IReaderResolver _readerResolver;
        private readonly IBankAccountRepository _bankAccountRepository;

        public CsvReaderService(
            ILogger<CsvReaderService> logger,
            IReaderResolver readerResolver,
            IBankAccountRepository bankAccountRepository)
        {
            _logger = logger;
            _readerResolver = readerResolver;
            _bankAccountRepository = bankAccountRepository;
        }

        public async Task<IEnumerable<FileStatusDTO>> ReadTransactionsFromFiles(IFormFileCollection csvFiles, short bankCode)
        {
            IBankEstatementReader reader = _readerResolver.GetBankEstatementReader(BankCode.FromValue<BankCode>(bankCode));
            var fileStatusResult = new List<FileStatusDTO>();

            foreach (var csv in csvFiles)
            {
                var fileStatus = new FileStatusDTO();
                fileStatus.FileName = csv.FileName;

                try
                {
                    Validate.IsTrue(csv.ContentType == "text/csv", "Only csv files are accepted");

                    Validate.IsTrue(csv.Length > 0, $"File {csv.FileName} is empty");

                    var parsedFile = reader.ProcessBankInformation(csv);

                    await PersistBankEstatementInformation(parsedFile, bankCode);

                    fileStatus.IsSuccessfullRead = true;
                    fileStatus.TransactionsSaved = parsedFile.Transactions.Count;
                    fileStatus.InitialDate = parsedFile.Transactions.First().Date;
                    fileStatus.FinalDate = parsedFile.Transactions.Last().Date;
                }
                catch (Exception ex)
                {
                    // TODO: create exception for file and reading cases, and another for database cases
                    _logger.LogWarning("Message: {0}", ex.Message);
                    fileStatus.IsSuccessfullRead = false;
                }
                finally
                {
                    fileStatusResult.Add(fileStatus);
                }
            }

            return fileStatusResult;
        }

        private async Task PersistBankEstatementInformation(StandardBankStatementDTO parsedFile, short bankCode)
        {
            var enumBankCode = BankCode.FromValue<BankCode>(bankCode);

            var bankAccount = await _bankAccountRepository.GetBy(parsedFile.AgencyNumber, parsedFile.AccountNumber, enumBankCode.Value);

            Validate.NotNull(bankAccount, 
                $"Conta bancária do banco {enumBankCode.Name}, agência: {parsedFile.AgencyNumber} e número: {parsedFile.AccountNumber} não foi cadastrada para o usuário ainda.");

            foreach (var data in parsedFile.Transactions)
            {
                // TODO: create mapping between Transaction and StandardTransaction
                var newTransaction = new Transaction(
                    data.Origin,
                    data.Date,
                    data.Amount);

                bankAccount.Transactions.Add(newTransaction);
            }

            await _bankAccountRepository.Update(bankAccount);
        }
    }
    /*NOTES:
     // TODO: Create custom exceptions: custom excepttions are managed through fileStatusDTO, all other must end up in BadRequest or 500 Internal Error*/
}
