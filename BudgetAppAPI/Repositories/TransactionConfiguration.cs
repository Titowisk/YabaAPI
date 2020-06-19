using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YabaAPI.Models;

namespace YabaAPI.Repositories
{
	public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
	{
		public void Configure(EntityTypeBuilder<Transaction> builder)
		{
			builder
				.ToTable("Transactions")
				.HasKey(t => t.Id);


			builder
				.Property(t => t.Id)
				.HasColumnName("TR_Id")
				.ValueGeneratedOnAdd()
				.IsRequired();

			builder
				.Property(t => t.Origin)
				.HasColumnName("TR_Origin")
				.HasMaxLength(100);

			builder
				.Property(t => t.Amount)
				.HasColumnName("TR_Amount")
				.HasColumnType("decimal(12,2)")
				.IsRequired();

			builder
				.Property(t => t.Date)
				.HasColumnName("TR_Date")
				.HasColumnType("date")
				.IsRequired();

			builder.Property(t => t.Category)
				.HasColumnName("TR_Category");
		}
	}
}
