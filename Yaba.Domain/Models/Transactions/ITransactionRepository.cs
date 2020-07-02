using System.Collections.Generic;

namespace Yaba.Domain.Models.Transactions
{
    public interface ITransactionRepository
    {
        Transaction GetById(long id);
        Transaction GetByIdWithBankAccount(long id);
        IEnumerable<Transaction> GetAll();
        void Create(Transaction entity);
        void Update(Transaction entity);
        void Delete(long id);
        void Delete(Transaction entity);
    }
}
