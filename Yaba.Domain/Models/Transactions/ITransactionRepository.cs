using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yaba.Domain.Models.Transactions
{
    public interface ITransactionRepository
    {
        Task<Transaction> GetById(object id);
        Task<Transaction> GetByIdWithBankAccount(long id);
        Task<IEnumerable<Transaction>> GetAll();
        void Insert(Transaction entity);
        void Update(Transaction entity);
        void Delete(Transaction entity);
        void DeleteRange(IEnumerable<Transaction> entities);
    }
}
