using Microsoft.EntityFrameworkCore;
using YabaAPI.Models;

namespace YabaAPI.Repositories
{
	public class DataContext : DbContext
	{
		public DbSet<Transaction> Transactions { get; set; }
		public DbSet<BankAccount> BankAccounts { get; set; }

		public DataContext(DbContextOptions<DataContext> options) : base(options)
		{	}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new TransactionConfiguration());

			modelBuilder.ApplyConfiguration(new BankAccountConfiguration());
		}
	}
}
