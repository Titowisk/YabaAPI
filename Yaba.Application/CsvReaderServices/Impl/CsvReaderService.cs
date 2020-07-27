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
using Yaba.Infrastructure.Persistence.UnitOfWork;
using Yaba.Tools.Validations;

namespace Yaba.Application.CsvReaderServices.Impl
{
    public class CsvReaderService : ICsvReaderService
    {
        private readonly UnitOfWork _uow;
        private readonly ILogger<CsvReaderService> _logger;
        private readonly IReaderResolver _readerResolver;
        private readonly IBankAccountRepository _bankAccountRepository;

        public CsvReaderService(
            UnitOfWork uow,
            ILogger<CsvReaderService> logger,
            IReaderResolver readerResolver,
            IBankAccountRepository bankAccountRepository)
        {
            _uow = uow;
            _logger = logger;
            _readerResolver = readerResolver;
            _bankAccountRepository = bankAccountRepository;
        }

        public async Task<IEnumerable<FileStatusDTO>> ReadTransactionsFromFiles(CsvFileReaderDTO dto)
        {
            IBankEstatementReader reader = _readerResolver.GetBankEstatementReader(BankCode.FromValue<BankCode>(dto.BankCode));
            var fileStatusResult = new List<FileStatusDTO>();

            foreach (var csv in dto.CsvFiles)
            {
                var fileStatus = new FileStatusDTO();
                fileStatus.FileName = csv.FileName;

                try
                {
                    Validate.IsTrue(csv.ContentType == "text/csv", "Only csv files are accepted");

                    Validate.IsTrue(csv.Length > 0, $"File {csv.FileName} is empty");

                    var parsedFile = reader.ProcessBankInformation(csv);

                    var bankAccount = await GetFilesOwnerBankAccount(dto.FilesOwnerId, parsedFile.AgencyNumber, parsedFile.AccountNumber, dto.BankCode);

                    await PersistBankEstatementInformation(parsedFile, bankAccount);

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

        private async Task<BankAccount> GetFilesOwnerBankAccount(int filesOwnerId, string agencyNumber, string accountNumber, short bankCode)
        {
            var bankAccount = await _bankAccountRepository.GetBy(agencyNumber, accountNumber, bankCode);

            Validate.NotNull(bankAccount,
                $"Conta bancária do banco {bankCode}, agência: {agencyNumber} e número: {accountNumber} não foi cadastrada para o usuário ainda.");

            Validate.IsTrue(filesOwnerId == bankAccount.UserId, "Acesso negado");

            return bankAccount;
        }

        private async Task PersistBankEstatementInformation(StandardBankStatementDTO parsedFile, BankAccount bankAccount)
        {
            foreach (var data in parsedFile.Transactions)
            {
                // TODO: create mapping between Transaction and StandardTransaction
                var newTransaction = new Transaction(
                    data.Origin,
                    data.Date,
                    data.Amount);

                bankAccount.Transactions.Add(newTransaction);
            }

            _bankAccountRepository.Update(bankAccount);
            Validate.IsTrue(await _uow.CommitAsync(), "Não foi possível salvar as transações lidas");
        }
    }
    /*NOTES:
     // TODO: Create custom exceptions: custom excepttions are managed through fileStatusDTO, all other must end up in BadRequest or 500 Internal Error*/
}
