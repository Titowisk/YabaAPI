using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Domain.Models.BankAccounts.Enumerations;
using Yaba.Domain.Models.Transactions;
using Yaba.Domain.Models.Transactions.Enumerations;
using Yaba.Domain.Models.Users;
using Yaba.Infrastructure.Security;

namespace Yaba.Infrastructure.Persistence.Context
{
    public static class Seeder
    {
        public static void Seed(this DataContext dataContext)
        {
            bool doesUserExist = dataContext.Users.Any(u => u.Email == "test-seed@gmail.com");
            if (doesUserExist)
                return;

            string encryptedPassword = SecurityManager.GeneratePbkdf2Hash("123Correct_");

            User testUser = new("test-user", "test-seed@gmail.com", encryptedPassword);
            dataContext.Users.Add(testUser);
            dataContext.SaveChanges();

            BankAccount account = new("123459789", "1234", BankCode.GENERICBANK, testUser.Id);
            dataContext.BankAccounts.Add(account);
            dataContext.SaveChanges();

            List<Transaction> transactions = new Faker<Transaction>()
                .RuleFor(t => t.Origin, f => f.Company.CompanyName())
                .RuleFor(t => t.Date, (f, t) => f.Date.Past(2, new DateTime(2024, 3, 15)))
                .RuleFor(t => t.Amount, (f, t) => f.Finance.Amount(3.00M, 467.88M))
                .RuleFor(t => t.BankAccountId, account.Id)
                .RuleFor(t => t.Category, f => f.Random.Enum<Category>())
                .RuleFor(t => t.Metadata, f => $"GenericBank_{Guid.NewGuid()}")
                .Generate(100)
                ;
            dataContext.Transactions.AddRange(transactions);

            dataContext.SaveChanges();
        }
    }
}
