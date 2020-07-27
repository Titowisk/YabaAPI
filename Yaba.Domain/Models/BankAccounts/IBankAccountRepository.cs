using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yaba.Domain.Models.BankAccounts
{
    public interface IBankAccountRepository
    {
        Task<IEnumerable<BankAccount>> GetAll();
        Task<BankAccount> GetById(object id);
        Task<BankAccount> GetBy(string agency, string number, short code);
        void Update(BankAccount entity);
        void Delete(BankAccount entity);
        void Insert(BankAccount entity);
    }
}
