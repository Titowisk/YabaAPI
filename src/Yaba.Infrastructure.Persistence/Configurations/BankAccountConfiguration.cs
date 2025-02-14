using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yaba.Domain.Models.BankAccounts;

namespace Yaba.Infrastructure.Persistence.Configurations
{
    public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            builder
                .ToTable("BankAccounts")
                .HasKey(bk => bk.Id);

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

            builder
                .Property(bk => bk.Code)
                .HasColumnName("BK_Code")
                .IsRequired();

            builder
                .Property(bk => bk.UserId)
                .HasColumnName("BK_UserId");

            builder
                .HasOne(bk => bk.User)
                .WithMany(u => u.BankAccounts)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
