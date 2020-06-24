using System.Collections.Generic;
using YabaAPI.Models;

namespace YabaAPI.Repositories.Contracts
{
    public interface ITransactionRepository
    {
        Transaction GetById(long id);
        Transaction GetByIdWithBankAccount(long id);
        IEnumerable<Transaction> GetAll();
        void Create(Transaction entity);
        void Update(Transaction entity);
        void Delete(long id);

        void Save();
    }
}
