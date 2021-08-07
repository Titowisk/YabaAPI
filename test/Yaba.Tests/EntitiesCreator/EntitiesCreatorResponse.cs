using Yaba.Domain.Models.BankAccounts;
using Yaba.Domain.Models.Users;

namespace Yaba.Tests.EntitiesCreator
{
    public class EntitiesCreatorResponse
    {
        public BankAccount BankAccount { get; set; }

        public User User { get; set; }
    }
}
