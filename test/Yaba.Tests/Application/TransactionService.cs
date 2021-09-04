using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Yaba.Application.TransactionServices;
using Yaba.Domain.Models.Transactions;
using Yaba.Domain.Models.Transactions.Enumerations;
using Yaba.Infrastructure.Persistence.Context;
using Yaba.Tests.EntitiesCreator.TransactionEntity;

namespace Yaba.Tests.Application
{
    // TODO: Test this service using TransactionBuilder
    public class TransactionService
    {
        private readonly string _sqlite_db_filename;

        public TransactionService()
        {
            this._sqlite_db_filename = this.GetType().Name;
        }

        [Fact]
        public async Task CategorizeTransactionsWithSimilarOriginsWithinAMonth_ChangeCategoriesOfTransactionsCorrectly()
        {
            var serviceProvider = DependencyInversion.DependencyContainer.GetServicesUsingSQLite(_sqlite_db_filename);
            const string origin = "similarOrigin";
            // given 10 transactions of a particular month
            //  and 4 of them have similar origins
            var response = TransactionBuilder.Create(serviceProvider)
                .AddManyExpenseTransactions(6, 2020, 10, Category.Food)
                .AddSingleTransaction(origin, new DateTime(2020, 10, 1), 1.0M)
                .AddSingleTransaction(origin, new DateTime(2020, 10, 2), 1.0M)
                .AddSingleTransaction(origin, new DateTime(2020, 10, 3), 1.0M)
                .AddSingleTransaction(origin, new DateTime(2020, 10, 4), 1.0M)
                .Build();

            // when the service CategorizeTransactionsWithSimilarOriginsWithinAMonth is called
            var transactionCategorizedByClient = response.Transactions.First(t => t.Origin == origin);
            transactionCategorizedByClient.Category = Category.Entertainment;

            var transactionService = (ITransactionService)serviceProvider.GetService(typeof(ITransactionService));
            await transactionService.CategorizeTransactionsWithSimilarOriginsWithinAMonth(transactionCategorizedByClient, (short)Category.Entertainment);

            // then all the 4 transactions with similar origin will be categorized with the same provided category
            var transactionRepository = (ITransactionRepository)serviceProvider.GetService(typeof(ITransactionRepository));
            var transactionsWithSimilarOrigin = await transactionRepository.GetByDateAndOrigin(new DateTime(2020, 10, 1), origin, response.BankAccount.Id);

            Assert.Equal(4, transactionsWithSimilarOrigin.Count());
            Assert.All(transactionsWithSimilarOrigin, t => Assert.Equal(origin, t.Origin));
            Assert.All(transactionsWithSimilarOrigin, t => Assert.Equal(Category.Entertainment, t.Category));
        }

        [Fact]
        public async Task CategorizeAllOtherTransactions_ChangeCategoriesOfTransactionsWithEqualOriginButWithDifferentCategory()
        {
            var serviceProvider = DependencyInversion.DependencyContainer.GetServicesUsingSQLite(_sqlite_db_filename);
            const string equal_origin = "some market expense";
            // given 10 transactions
            //  1 was categorized by client (Category.Food) and has origin: "some market expense"
            //  5 of them with equal origin but with different category
            //  4 of them with different origin and with different category
            var response = TransactionBuilder.Create(serviceProvider)
                                            .AddSingleTransaction(equal_origin, new DateTime(2020, 10, 1), 1.0M, Category.Food)
                                            .AddManyExpenseTransactions(5,category: Category.Education, origin: equal_origin)
                                            .AddManyExpenseTransactions(4, category: Category.Entertainment, origin: "Sweet Sweeties 123")
                                            .Build();



            // when the service CategorizeAllOtherTransactions is called for a categorized transaction
            var categorizedTransaction = response.Transactions.First(t => t.Origin == equal_origin && t.Category == Category.Food);

            var transactionService = (ITransactionService)serviceProvider.GetService(typeof(ITransactionService));
            await transactionService.CategorizeAllOtherTransactions(categorizedTransaction.Id);

            // then it should categorize transactions that have equal origin and different category
            var dataContext = (DataContext)serviceProvider.GetService(typeof(DataContext));
            var transactionsUnderTest = dataContext.Set<Transaction>();

            var categorizedTransactions = transactionsUnderTest
                .Where(t => t.Category == Category.Food && t.Origin == equal_origin)
                .ToList();

            var untouchedTransactions = transactionsUnderTest
                .Where(t => t.Category == Category.Entertainment)
                .ToList();

            Assert.Equal(6, categorizedTransactions.Count); // 1 by client + 5 by service
            Assert.Equal(4, untouchedTransactions.Count);
            Assert.All(categorizedTransactions, t => Assert.Equal(categorizedTransaction.Category, t.Category));
            Assert.All(untouchedTransactions, t => Assert.True(categorizedTransaction.Category != t.Category));
        }
    }
}
