using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;
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
using Yaba.Infrastructure.DTO;
using Yaba.Infrastructure.Persistence.Context;
using Yaba.Infrastructure.Persistence.Repositories;
using Yaba.Infrastructure.Persistence.UnitOfWork;

namespace Yaba.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<JwtConfig>(Configuration.GetSection("JwtConfig"));

            services.AddControllers()
                .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
            services.AddDbContext<DataContext>(opt =>
                opt.UseSqlServer(Configuration.GetConnectionString("SqlServerDB")));
            services.AddScoped<UnitOfWork>();

            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IBankAccountRepository, BankAccountRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBankAccountService, BankAccountService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<ICsvReaderService, CsvReaderService>();

            services.AddSingleton<IReaderResolver, ReaderResolver>();
            services.AddTransient<BradescoReader>();

            var secretKey = Configuration.GetSection("JwtConfig:SecretKey").Value;
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            ConfigureTokenValidation(services, securityKey);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            // $Env:ASPNETCORE_ENVIRONMENT = "Development"

            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureTokenValidation(IServiceCollection services, SecurityKey securityKey)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = "Yaba API",
                ValidateAudience = true,
                ValidAudience = "Yaba API",
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey
            };

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = _env.IsDevelopment();
                    options.SaveToken = true;
                    options.TokenValidationParameters = tokenValidationParameters;
                });
        }
    }
}
