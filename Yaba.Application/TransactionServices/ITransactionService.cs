﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yaba.Infrastructure.DTO;
using Yaba.Domain.Models.Transactions;
using Yaba.Infrastructure.DTO.Transactions;

namespace Yaba.Application.TransactionServices
{
    public interface ITransactionService : IDisposable
    {
        Task<TransactionsDateFilterResponseDTO> GetByMonth(GetUserTransactionsByMonthDTO dto);
        Task Create(CreateUserTransactionDTO dto);
        Task<IEnumerable<ExistentTransactionsDatesResponseDTO>> GetExistentTransactionsDatesByUser(GetTransactionDatesDTO dto);
        Task CategorizeAllTransactionsWithSimilarOriginsToTransactionSentByClient(CategorizeUserTransactionsDTO dto);
        Task CategorizeTransactionsWithSimilarOriginsWithinAMonth(Transaction transaction, short categoryId);
        Task CategorizeTransactionsWithSimilarOrigin(CategorizeTransactionsQueryDTO dtoQuery, CategorizeTransactionsBodyDTO dtoBody);

        Task CategorizeTransactionsUsingCategoryWorker(CategorizeTransactionsQueryDTO dtoQuery, CategorizeTransactionsBodyDTO dtoBody);
        Task CategorizeAllOtherTransactions(long transactionId);
        Task CategorizeAllOtherTransactionsOutsideMonth(long transactionId);

        Task GenerateRandomizedDataForGenericBank(GenerateDataDTO dto);

        IEnumerable<CategoryDTO> GetCategories();
    }
}
