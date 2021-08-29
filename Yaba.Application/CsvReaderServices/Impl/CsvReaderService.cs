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
        private readonly ITransactionRepository _transactionRepository;
        private readonly IBankAccountRepository _bankAccountRepository;

        public CsvReaderService(
            UnitOfWork uow,
            ILogger<CsvReaderService> logger,
            ITransactionRepository transactionRepository,
            IReaderResolver readerResolver,
            IBankAccountRepository bankAccountRepository)
        {
            _uow = uow;
            _logger = logger;
            _transactionRepository = transactionRepository;
            _readerResolver = readerResolver;
            _bankAccountRepository = bankAccountRepository;
        }

        public async Task<IEnumerable<FileStatusDTO>> ReadTransactionsFromFiles(CsvFileReaderDTO dto)
        {
            var bankAccount = await _bankAccountRepository.GetById(dto.BankAccountId);
            Validate.IsTrue(bankAccount.UserId == dto.FilesOwnerId, "Bank account not found");

            IBankEstatementReader reader = _readerResolver.GetBankEstatementReader(BankCode.FromValue<BankCode>(bankAccount.Code));
            var fileStatusResult = new List<FileStatusDTO>();

            // TODO: Use Refactoring book to change this loop into pipeline (create a test before): Replace Loop With A Pipeline-231
            foreach (var csv in dto.CsvFiles)
            {
                var fileStatus = new FileStatusDTO
                {
                    FileName = csv.FileName
                };

                try
                {
                    Validate.IsTrue(csv.ContentType == "text/csv", "Only csv files are accepted");

                    Validate.IsTrue(csv.Length > 0, $"File {csv.FileName} is empty");

                    var parsedFile = reader.ProcessBankInformation(csv);
                    fileStatus.TransactionsRead = parsedFile.Transactions.Count;

                    //var bankAccount = await GetFilesOwnerBankAccount(dto.FilesOwnerId, parsedFile.AgencyNumber, parsedFile.AccountNumber, dto.BankAccountId);

                    RemoveExistentTransactions(parsedFile);

                    if(parsedFile.Transactions.Count > 0)
                        await PersistBankEstatementInformation(parsedFile, bankAccount);


                    fileStatus.IsSuccessfullRead = true;
                    fileStatus.TransactionsSaved = parsedFile.Transactions.Count;
                    // TODO: Get initial and final date from file
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

        public void Dispose()
        {
            _uow.Dispose(); // TODO: does it really dispose all resources from the context (BankAccount, User, Transaction ) ??
        }

        #region Priv methods
        private void RemoveExistentTransactions(StandardBankStatementDTO parsedFile)
        {
            parsedFile.Transactions
                .RemoveAll(t => _transactionRepository.DoesTransactionExists(t.TransactionUniqueHash).Result);
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
                    data.Amount,
                    bankAccount.Id)
                {
                    Metadata = data.TransactionUniqueHash
                };

                bankAccount.Transactions.Add(newTransaction);
            }

            _bankAccountRepository.Update(bankAccount);
            Validate.IsTrue(await _uow.CommitAsync(), "Não foi possível salvar as transações lidas");

            // TODO: send message
            await CategorizeTransactionsBasedOnUserHistory(bankAccount.Transactions.Select(t => t.Id).ToArray(), bankAccount);
        }

        // TODO: make it a transactionService and create endpoint to client use
        private async Task CategorizeTransactionsBasedOnUserHistory(long[] transactionsIds, BankAccount bankAccount)
        {
            var transactions = await _transactionRepository.GetByIds(transactionsIds);
            if (!transactions.Any())
                return;

            var referenceDate = transactions.Select(t => t.Date).First();
            var recentTransactionsWithCategory = await _transactionRepository.GetPredecessors(referenceDate, bankAccount.Id);
            if (!recentTransactionsWithCategory.Any())
                return;

            List<Transaction> categorizedTransactions = CategorizeTransactions(transactions, recentTransactionsWithCategory);

            _transactionRepository.UpdateRange(categorizedTransactions);
            Validate.IsTrue(await _uow.CommitAsync(), "Não foi possível atualizar as transações salvas");
        }

        private List<Transaction> CategorizeTransactions(IEnumerable<Transaction> transactions, IEnumerable<Transaction> recentTransactionsWithCategory)
        {
            var categorizedTransactions = new List<Transaction>();
            var currentOrigin = string.Empty;
            Transaction transactionWithCategory = null;
            foreach (var currentTransaction in transactions.OrderBy(t => t.Origin))
            {
                if (currentTransaction.Origin != currentOrigin)
                {
                    transactionWithCategory = recentTransactionsWithCategory.FirstOrDefault(t => t.Origin == currentTransaction.Origin);
                    if (transactionWithCategory is null) continue;
                    currentOrigin = currentTransaction.Origin;
                }
                currentTransaction.Category = transactionWithCategory.Category;
                categorizedTransactions.Add(currentTransaction);
            }

            return categorizedTransactions;
        }
        #endregion
    }
    /*NOTES:
     // TODO: Create custom exceptions: custom excepttions are managed through fileStatusDTO, all other must end up in BadRequest or 500 Internal Error*/
}
