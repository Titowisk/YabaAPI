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
			modelBuilder.Entity<Transaction>()
				.ToTable("Transactions")
				.HasKey(t => t.Id);


			modelBuilder.Entity<Transaction>()
				.Property(t => t.Id)
				.HasColumnName("TR_Id")
				.ValueGeneratedOnAdd()
				.IsRequired();

			modelBuilder.Entity<Transaction>()
				.Property(t => t.Origin)
				.HasColumnName("TR_Origin")
				.HasMaxLength(100);

			modelBuilder.Entity<Transaction>()
				.Property(t => t.Amount)
				.HasColumnName("TR_Amount")
				.HasColumnType("decimal(12,2)")
				.IsRequired();

			modelBuilder.Entity<Transaction>()
				.Property(t => t.Date)
				.HasColumnName("TR_Date")
				.HasColumnType("date")
				.IsRequired();

			// Too much code in this DataContext
			modelBuilder.Entity<BankAccount>()
				.ToTable("BankAccounts")
				.HasKey(bk => bk.Id);

			modelBuilder.Entity<BankAccount>()
				.HasMany(bk => bk.Transactions)
				.WithOne(t => t.BankAccount);

			modelBuilder.Entity<BankAccount>()
				.Property(bk => bk.Id)
				.HasColumnName("BK_Id")
				.IsRequired();

			modelBuilder.Entity<BankAccount>()
				.Property(bk => bk.Number)
				.HasColumnName("BK_Number")
				.IsRequired();

			modelBuilder.Entity<BankAccount>()
				.Property(bk => bk.Agency)
				.HasColumnName("BK_Agency")
				.IsRequired();
		}
	}
}
