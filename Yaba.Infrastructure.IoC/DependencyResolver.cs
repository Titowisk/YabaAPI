using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

namespace Yaba.Infrastructure.IoC
{
    public static class DependencyResolver
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(opt =>
                opt.UseSqlServer(configuration.GetConnectionString("SqlServerDB")));
            services.AddScoped<UnitOfWork>();

            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IBankAccountRepository, BankAccountRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBankAccountService, BankAccountService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<ICsvReaderService, CsvReaderService>();

            services.AddSingleton<IReaderResolver, ReaderResolver>();
            services.AddScoped<BradescoReader>();
            services.AddScoped<NuBankReader>();

            services.AddScoped<IQueueMessageService, QueueMessageService>();

        }
    }
}
