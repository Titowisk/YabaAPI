using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yaba.Domain.Models.Users;

namespace Yaba.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
                .ToTable("Users")
                .HasKey(u => u.Id);

            builder
                .Property(u => u.Id)
                .HasColumnName("US_Id")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder
                .Property(u => u.Name)
                .HasColumnName("US_Name")
                .HasMaxLength(256)
                .IsRequired();

            builder
                .Property(u => u.Email)
                .HasColumnName("US_Email")
                .HasMaxLength(256)
                .IsRequired();

            builder
                .Property(u => u.Password)
                .HasColumnName("US_Password")
                .HasMaxLength(256)
                .IsRequired();
        }
    }
}
