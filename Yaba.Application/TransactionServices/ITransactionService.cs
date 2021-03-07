using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yaba.Infrastructure.DTO;

namespace Yaba.Application.TransactionServices
{
    public interface ITransactionService : IDisposable
    {
        Task<IEnumerable<TransactionsDateFilterResponseDTO>> GetByMonth(GetUserTransactionsByMonthDTO dto);
        Task Create(CreateUserTransactionDTO dto);
        Task<IEnumerable<ExistentTransactionsDatesResponseDTO>> GetExistentTransactionsDatesByUser(GetTransactionDatesDTO dto);
        Task CategorizeAllTransactionsWithSimilarOrigins(CategorizeUserTransactionsDTO dto);
        Task CategorizeAllOtherTransactions(long transactionId);

        IEnumerable<CategoryDTO> GetCategories();
    }
}
