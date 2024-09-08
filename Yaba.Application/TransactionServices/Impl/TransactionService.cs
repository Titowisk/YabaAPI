using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Domain.Models.BankAccounts.Enumerations;
using Yaba.Domain.Models.Transactions;
using Yaba.Domain.Models.Transactions.Enumerations;
using Yaba.Infrastructure.AzureStorageQueue.Contracts;
using Yaba.Infrastructure.DTO;
using Yaba.Infrastructure.DTO.Transactions;
using Yaba.Infrastructure.Persistence.UnitOfWork;
using Yaba.Tools.Validations;
using Transaction = Yaba.Domain.Models.Transactions.Transaction;

namespace Yaba.Application.TransactionServices.Impl
{
    public class TransactionService : ITransactionService
    {
        private readonly IQueueMessageService _queueMessageService;
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly UnitOfWork _uow;

        public TransactionService(
            UnitOfWork uow,
            IQueueMessageService queueMessageService,
            ITransactionRepository transactionRepository,
            IBankAccountRepository bankAccountRepository)
        {
            _queueMessageService = queueMessageService;
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

        public async Task<TransactionsDateFilterResponseDTO> GetByMonth(GetUserTransactionsByMonthDTO dto)
        {
            var transactions = await _transactionRepository.GetByMonthBankAccountUser(dto.Year, dto.Month, dto.BankAccountId, dto.UserId); ;
            Validate.IsTrue(transactions.Count > 0, "Não foram encontradas transações");

            return CalculateTransactionsSummary(transactions);
        }

        public IEnumerable<CategoryDTO> GetCategories()
        {
            var categoryType = typeof(Category);
            var categories = new List<CategoryDTO>();
            foreach (var value in Enum.GetValues(categoryType))
            {
                var category = new CategoryDTO
                {
                    Key = (short)value,
                    Value = (short)value,
                    Text = Enum.GetName(categoryType, value)
                };
                categories.Add(category);
            }

            return categories;
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

        [Obsolete("Does not work anymero, use CategorizeTransactionsUsingCategoryWorker")]
        public async Task CategorizeAllTransactionsWithSimilarOriginsToTransactionSentByClient(CategorizeUserTransactionsDTO dto)
        {
            var transactionSentByClient = await _transactionRepository.GetById(dto.TransactionId);
            Validate.NotNull(transactionSentByClient, "This Transaction doesn't exist");

            var bankAccount = await _bankAccountRepository.GetById(transactionSentByClient.BankAccountId);
            Validate.NotNull(bankAccount, "Access Denied");

            Validate.IsTrue(bankAccount.UserId == dto.UserId, "Access Denied");

            await CategorizeTransactionsWithSimilarOriginsWithinAMonth(transactionSentByClient, dto.CategoryId);

            Validate.IsTrue(await _uow.CommitAsync(), "Ocorreu um problema na criação da transação");

            var message = transactionSentByClient.Id.ToString(); // TODO: create message class with a good name
            _queueMessageService.SendMessage(message);
            // TODO: _bus.RaiseEvent(new UserTransactionsWereCategorizedEvent(dto))
        }

        public async Task CategorizeAllOtherTransactions(long transactionId)
        {
            Transaction transaction = await _transactionRepository.GetById(transactionId);
            Validate.NotNull(transaction);

            var similarTransactions = await _transactionRepository.GetAllOtherTransactions(transaction);

            foreach (var tr in similarTransactions)
            {
                tr.Category = transaction.Category;
            }

            _transactionRepository.UpdateRange(similarTransactions);

            Validate.IsTrue(await _uow.CommitAsync(), "Ocorreu um problema na categorização das transações");
        }

        /// <summary>
        /// Categorize all transactions with similar origin in one action
        /// </summary>
        /// <param name="dtoQuery"></param>
        /// <param name="dtoBody"></param>
        /// <returns></returns>
        public async Task CategorizeTransactionsWithSimilarOrigin(CategorizeTransactionsQueryDTO dtoQuery, CategorizeTransactionsBodyDTO dtoBody)
        {
            Validate.NotNullOrEmpty(dtoQuery.Origin, "transaction origin not provided");
            Validate.IsTrue(dtoQuery.Origin.Length > 2, "origin too short");

            var transactions = await _transactionRepository.GetByDateAndOrigin(dtoQuery.UserId, dtoQuery.BankAccountId, dtoQuery.Origin, dtoQuery.Year, dtoQuery.Month);

            foreach (var tr in transactions)
            {
                tr.Category = (Category)dtoBody.CategoryId;
            }

            _transactionRepository.UpdateRange(transactions);

            Validate.IsTrue(await _uow.CommitAsync(), "Ocorreu um problema na categorização das transações");
        }

        public async Task CategorizeTransactionsUsingCategoryWorker(CategorizeTransactionsQueryDTO dtoQuery, CategorizeTransactionsBodyDTO dtoBody)
        {
            Validate.NotNullOrEmpty(dtoQuery.Origin, "transaction origin not provided");
            Validate.IsTrue(dtoQuery.Origin.Length > 2, "origin too short");

            var transaction =  await _transactionRepository.GetById(dtoQuery.TransactionId);

            var bankAccount = await _bankAccountRepository.GetById(transaction.BankAccountId);
            Validate.NotNull(bankAccount, "Access Denied");

            Validate.IsTrue(bankAccount.UserId == dtoQuery.UserId, "Access Denied");

            await CategorizeTransactionsWithSimilarOriginsWithinAMonth(transaction, dtoBody.CategoryId);

            // send message to worker
        }

        public async Task CategorizeTransactionsWithSimilarOriginsWithinAMonth(Transaction transaction, short categoryId)
        {
            var similarTransactions = await _transactionRepository.GetByDateAndOrigin(transaction.Date, transaction.Origin, (int)transaction.BankAccountId);

            if (!similarTransactions.Any())
                return;

            foreach (var t in similarTransactions)
            {
                t.Category = (Category)categoryId;
            }

            _transactionRepository.UpdateRange(similarTransactions);
            Validate.IsTrue(await _uow.CommitAsync(), "Ocorreu um problema na categorização das transações");

        }

        public async Task GenerateRandomizedDataForGenericBank(GenerateDataDTO dto)
        {
            var bankAccount = await _bankAccountRepository.GetById(dto.BankAccountId);
            Validate.NotNull(bankAccount);
            Validate.IsTrue(bankAccount.Code == BankCode.GENERICBANK.Value, "Access Denied");

            Validate.IsTrue(bankAccount.UserId == dto.UserId, "Access Denied");

            var incomeQuantity = dto.Quantity / 3;
            var fakeExpenseTransactions = new Faker<Transaction>()
                .RuleFor(t => t.BankAccountId, dto.BankAccountId)
                .RuleFor(t => t.Date, f => new DateTime(dto.Year, dto.Month, f.Random.Int(1, 27)) )
                .RuleFor(t => t.Category, f => f.PickRandom<Category>())
                .RuleFor(t => t.Amount, f => f.Finance.Amount() * -1)
                .RuleFor(t => t.Origin, f => f.Company.CompanyName())
                .RuleFor(t => t.Metadata, f => $"GenericBank_{Guid.NewGuid()}")
                .Generate(dto.Quantity - incomeQuantity);

            var fakeIncomeTransactions = new Faker<Transaction>()
                .RuleFor(t => t.BankAccountId, dto.BankAccountId)
                .RuleFor(t => t.Date, f => new DateTime(dto.Year, dto.Month, f.Random.Int(1, 27)))
                .RuleFor(t => t.Category, f => Category.Income)
                .RuleFor(t => t.Amount, f => f.Finance.Amount())
                .RuleFor(t => t.Origin, f => "Some Income")
                .RuleFor(t => t.Metadata, f => $"GenericBank_{Guid.NewGuid()}")
                .Generate(incomeQuantity);

            var fakeTransactions = fakeExpenseTransactions.Union(fakeIncomeTransactions);
            _transactionRepository.InsertRange(fakeTransactions);

            Validate.IsTrue(await _uow.CommitAsync(), "Ocorreu um problema na criação das transações");
        }

        public void Dispose()
        {
            _uow.Dispose(); // TODO: does it really dispose all resources from the context (BankAccount, User, Transaction ) ??
        }

        #region Priv Methods

        

        private static TransactionsDateFilterResponseDTO CalculateTransactionsSummary(IEnumerable<TransactionsResponseDTO> transactions)
        {
            decimal totalIncome = 0;
            decimal totalExpense = 0;
            foreach (var tr in transactions)
            {
                if (tr.Amount > 0)
                    totalIncome += tr.Amount;
                else
                    totalExpense += tr.Amount;
            }

            var totalVolume = totalIncome + Math.Abs(totalExpense);

            return new TransactionsDateFilterResponseDTO
            {
                Transactions = transactions,
                TotalVolume = totalVolume,
                TotalExpense = totalExpense,
                TotalIncome = totalIncome,
                IncomePercentage = Math.Round((totalIncome / totalVolume) * 100, 1),
                ExpensePercentage = Math.Round((Math.Abs(totalExpense) / totalVolume) * 100, 1)
            };
        }
        #endregion
    }
}
