using Microsoft.EntityFrameworkCore;
using YabaAPI.Models;

namespace YabaAPI.Repositories
{
	public class DataContext : DbContext
	{
		public DbSet<Transaction> Transactions { get; set; }

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
		}
	}
}
