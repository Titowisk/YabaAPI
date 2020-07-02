using Microsoft.EntityFrameworkCore;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Domain.Models.Transactions;

namespace Yaba.Infrastructure.Persistence.Context
{
    public class DataContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());

            modelBuilder.ApplyConfiguration(new BankAccountConfiguration());
        }
    }
}
