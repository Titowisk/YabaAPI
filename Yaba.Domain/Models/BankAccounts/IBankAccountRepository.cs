using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yaba.Domain.Models.BankAccounts;

namespace Yaba.Domain.Models
{
    public interface IBankAccountRepository : IAsyncDisposable
    {
        Task<IEnumerable<BankAccount>> GetAll();
        Task<BankAccount> GetById(int id);
        BankAccount? Find(BankAccount bankAccount);
        Task Update(BankAccount entity);
        Task Delete(int id);
        Task Delete(BankAccount id);
        Task Create(BankAccount entity);
        Task<bool> Exists(int id);
    }
}
