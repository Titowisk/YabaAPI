using Microsoft.EntityFrameworkCore;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Domain.Models.Transactions;
using Yaba.Domain.Models.Users;
using Yaba.Infrastructure.Persistence.Configurations;

namespace Yaba.Infrastructure.Persistence.Context
{
    /// <summary>
    /// https://www.twilio.com/en-us/blog/containerize-your-sql-server-with-docker-and-aspnet-core-with-ef-core
    /// To run the application locally using a SQL Server Container
    /// docker run -it -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=A&VeryComplex123Password" -p 1433:1433 --name sql-server-2022-debug mcr.microsoft.com/mssql/server:2022-latest
    /// dotnet ef database update --startup-project "Yaba.WebApi/Yaba.WebApi.csproj" --project "Yaba.Infrastructure.Persistence/Yaba.Infrastructure.Persistence.csproj" --verbose -- --environment Development
    /// </summary>
    public class DataContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<User> Users { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());

            modelBuilder.ApplyConfiguration(new BankAccountConfiguration());

            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
