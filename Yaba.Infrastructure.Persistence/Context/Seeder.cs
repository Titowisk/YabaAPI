using System;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Domain.Models.BankAccounts.Enumerations;
using Yaba.Domain.Models.Transactions;
using Yaba.Domain.Models.Users;

namespace Yaba.Infrastructure.Persistence.Context
{
    public static class Seeder
    {
        public static void Seed(this DataContext dataContext)
        {
            // create user
            User testUser = new("test-user", "test@gmail.com", "123Correct_");
            dataContext.Users.Add(testUser);
            dataContext.SaveChanges();

            // create user bank
            BankAccount account = new("123459789", "1234", BankCode.GENERICBANK, testUser.Id);
            dataContext.BankAccounts.Add(account);
            dataContext.SaveChanges();

            // create 100 transactions
            Transaction transaction = new("hiper teste", DateTime.Now, 25.2M, account.Id);
            dataContext.Transactions.Add(transaction);

            dataContext.SaveChanges();
        }
    }
}
