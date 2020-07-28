using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Yaba.Infrastructure.DTO;
using Transaction = Yaba.Domain.Models.Transactions.Transaction;

namespace Yaba.Application.TransactionServices
{
    public interface ITransactionService : IDisposable
    {
        Task Delete(DeleteUserTransactionDTO dto);
        Task DeleteBatchBetween(DeleteUserTransactionBatchDTO dto);
        Task<IEnumerable<Transaction>> GetByMonth(GetUserTransactionsByMonthDTO dto);
        Task Create(CreateUserTransactionDTO dto);
    }
}
