﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YabaAPI.Models;

namespace YabaAPI.Repositories
{
	public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
	{
		public void Configure(EntityTypeBuilder<BankAccount> builder)
		{
			builder
				.ToTable("BankAccounts")
				.HasKey(bk => bk.Id);

			builder
				.HasMany(bk => bk.Transactions)
				.WithOne(t => t.BankAccount);

			builder
				.Property(bk => bk.Id)
				.HasColumnName("BK_Id")
				.IsRequired();

			builder
				.Property(bk => bk.Number)
				.HasColumnName("BK_Number")
				.IsRequired();

			builder
				.Property(bk => bk.Agency)
				.HasColumnName("BK_Agency")
				.IsRequired();
		}
	}
}