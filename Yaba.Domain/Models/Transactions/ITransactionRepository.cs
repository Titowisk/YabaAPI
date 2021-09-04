using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yaba.Infrastructure.DTO;

namespace Yaba.Domain.Models.Transactions
{
    public interface ITransactionRepository
    {
        Task<bool> DoesTransactionExists(string hash);
        Task<IEnumerable<DateDTO>> GetDatesByUser(int userId, int bankAccountId);
        Task<Transaction> GetById(object id);
        Task<IEnumerable<Transaction>> GetByIds(long[] ids);
        Task<Transaction> GetByIdWithBankAccount(long id);
        Task<IEnumerable<Transaction>> GetAll();
        Task<ICollection<TransactionsResponseDTO>> GetByMonthBankAccountUser(short year, short month, int bankAccountId, int userId);
        Task<IEnumerable<Transaction>> GetByDateAndOrigin(DateTime date, string origin, int bankAccountId);
        Task<IEnumerable<Transaction>> GetPredecessors(DateTime fromDateUntilToday, int bankAccountId);
        Task<IEnumerable<Transaction>> GetAllOtherTransactions(Transaction recentlyUpdatedtransaction);

        void Insert(Transaction entity);
        void InsertRange(IEnumerable<Transaction> transactions);
        void Update(Transaction entity);
        void Delete(Transaction entity);
        void DeleteRange(IEnumerable<Transaction> entities);
        void UpdateRange(IEnumerable<Transaction> entities);
    }
}
