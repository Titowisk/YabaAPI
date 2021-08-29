using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaba.Application.BankAccountServices;
using Yaba.Application.BankAccountServices.Impl;
using Yaba.Application.BankStatementReaders;
using Yaba.Application.BankStatementReaders.ReaderResolver;
using Yaba.Application.CsvReaderServices;
using Yaba.Application.CsvReaderServices.Impl;
using Yaba.Application.TransactionServices;
using Yaba.Application.TransactionServices.Impl;
using Yaba.Application.UserServices;
using Yaba.Application.UserServices.Impl;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Domain.Models.Transactions;
using Yaba.Domain.Models.Users;
using Yaba.Infrastructure.AzureStorageQueue.Contracts;
using Yaba.Infrastructure.AzureStorageQueue.Implementations;
using Yaba.Infrastructure.Persistence.Context;
using Yaba.Infrastructure.Persistence.Repositories;
using Yaba.Infrastructure.Persistence.UnitOfWork;

namespace Yaba.Tests.DependencyInversion
{
    public static class DependencyContainer
    {
        public static IServiceProvider GetServicesUsingSQLite(string dbFileName)
        {
            if (string.IsNullOrEmpty(dbFileName)) throw new Exception("dbFileName must be filled (preferable as the class name of the test)");

            IServiceCollection services = new ServiceCollection();

            var sqliteOptions = new DbContextOptionsBuilder<DataContext>().UseSqlite($"Filename={dbFileName}").Options;
            //services.AddDbContext<DataContext>(s => new DataContext(sqliteOptions));
            //var sqliteContext = new DataContext(sqliteOptions);
            //sqliteContext.Database.EnsureDeleted();
            //sqliteContext.Database.EnsureCreated();
            services.AddScoped(_ => {
                var context = new DataContext(sqliteOptions);
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                return context;
            });

            GetDomainCollection(services);
            GetApplicationCollection(services);
            GetInfrastructureCollection(services);

            return services.BuildServiceProvider();
        }

        private static void GetInfrastructureCollection(IServiceCollection services)
        {
            services.AddScoped(_ => new Mock<IQueueMessageService>().Object);
            services.AddScoped(_ => new Mock<ILogger<CsvReaderService>>().Object);
            services.AddScoped(_ => new Mock<ILogger<NuBankReader>>().Object);
        }

        private static void GetApplicationCollection(IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBankAccountService, BankAccountService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<ICsvReaderService, CsvReaderService>();

            services.AddSingleton<IReaderResolver, ReaderResolver>();
            services.AddScoped<BradescoReader>();
            services.AddScoped<NuBankReader>();
        }

        private static void GetDomainCollection(IServiceCollection services)
        {
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IBankAccountRepository, BankAccountRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<UnitOfWork>();
        }
    }
}
