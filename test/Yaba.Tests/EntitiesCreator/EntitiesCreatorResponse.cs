using System.Collections.Generic;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Domain.Models.Transactions;
using Yaba.Domain.Models.Users;

namespace Yaba.Tests.EntitiesCreator
{
    public class EntitiesCreatorResponse
    {
        public BankAccount BankAccount { get; set; }
        public List<BankAccount> BankAccounts { get; set; }
        public List<Transaction> Transactions { get; set; }
        public User User { get; set; }
    }
}
