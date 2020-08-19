using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Domain.Models.Transactions;
using Yaba.Domain.Models.Transactions.Enumerations;
using Yaba.Infrastructure.DTO;
using Yaba.Infrastructure.Persistence.UnitOfWork;
using Yaba.Tools.Validations;
using Transaction = Yaba.Domain.Models.Transactions.Transaction;

namespace Yaba.Application.TransactionServices.Impl
{
    public class TransactionService : ITransactionService
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly UnitOfWork _uow;

        public TransactionService(
            UnitOfWork uow,
            ITransactionRepository transactionRepository,
            IBankAccountRepository bankAccountRepository)
        {
            _bankAccountRepository = bankAccountRepository;
            _transactionRepository = transactionRepository;
            _uow = uow;
        }

        public async Task Create(CreateUserTransactionDTO dto)
        {
            var bankAccount = await _bankAccountRepository.GetById(dto.BankAccountId);
            Validate.IsTrue(bankAccount != null, "A conta bancária fornecida não foi encontrada");
            Validate.IsTrue(bankAccount.UserId == dto.UserId, "Acesso negado");

            // TODO: Validations?
            var transaction = new Transaction(dto.Origin, dto.Date, dto.Amount, dto.BankAccountId);

            _transactionRepository.Insert(transaction);

            Validate.IsTrue(await _uow.CommitAsync(), "Ocorreu um problema na criação da transação");
        }

        public async Task<IEnumerable<TransactionsDateFilterResponseDTO>> GetByMonth(GetUserTransactionsByMonthDTO dto)
        {
            var transactions = await _transactionRepository.GetByMonthBankAccountUser(dto.Year, dto.Month, dto.BankAccountId, dto.UserId); ;
            Validate.IsTrue(transactions.Count > 0, "Não foram encontradas transações");

            return transactions;
        }

        public async Task<IEnumerable<ExistentTransactionsDatesResponseDTO>> GetExistentTransactionsDatesByUser(GetTransactionDatesDTO dto)
        {
            var dates = await _transactionRepository.GetDatesByUser(dto.UserId, dto.BankaccountId);

            var existentDates = new List<ExistentTransactionsDatesResponseDTO>();

            foreach (var date in dates)
            {
                var existentDate = existentDates.FirstOrDefault(d => d.Year == date.Year);
                if (existentDate == null)
                {
                    existentDate = new ExistentTransactionsDatesResponseDTO()
                    {
                        Year = date.Year,
                    };

                    existentDates.Add(existentDate);
                }
                existentDate.Months.Add(date.Month);
            }

            return existentDates;
        }

        public async Task CategorizeAllTransactionsWithSimilarOrigins(CategorizeUserTransactionsDTO dto)
        {
            var transactionToUpdate = await _transactionRepository.GetById(dto.TransactionId);
            Validate.NotNull(transactionToUpdate, "This Transaction doesn't exist");

            var bankAccount = await _bankAccountRepository.GetById(transactionToUpdate.BankAccountId);
            Validate.NotNull(bankAccount, "Access Denied");

            Validate.IsTrue(bankAccount.UserId == dto.UserId, "Access Denied");

            _transactionRepository.Update(transactionToUpdate);

            await UpdateAllTransactionsWithSimilarOriginsByMonth(transactionToUpdate, dto.CategoryId);

            Validate.IsTrue(await _uow.CommitAsync(), "Ocorreu um problema na criação da transação");

            // TODO: _bus.RaiseEvent(new UserTransactionsWereCategorizedEvent(dto))
        }


        public void Dispose()
        {
            _uow.Dispose(); // TODO: does it really dispose all resources from the context (BankAccount, User, Transaction ) ??
        }

        #region Priv Methods

        private async Task UpdateAllTransactionsWithSimilarOriginsByMonth(Transaction transaction, short categoryId)
        {
            var similarTransactions = await _transactionRepository.GetByDateAndOrigin(transaction.Date, transaction.Origin, (int)transaction.BankAccountId);

            if (similarTransactions.Count() == 0)
                return;

            foreach (var t in similarTransactions)
            {
                t.Category = (Category)categoryId;
            }

            _transactionRepository.UpdateRange(similarTransactions);
        }

        #endregion
    }
}
