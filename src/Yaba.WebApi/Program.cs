using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Yaba.Infrastructure.DTO;
using Yaba.Infrastructure.IoC;
using Yaba.Infrastructure.Persistence.Context;
using Yaba.WebApi.Middlewares;

/// Reference
/// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-8.0#default-application-configuration-sources
/// 

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Logging.AddConsole();

ConfigServiceCollection(builder);

// <
WebApplication app = builder.Build();
ConfigWebApplication(app);


static void ConfigWebApplication(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();

        var yabaContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        yabaContext.Database.EnsureCreated();

        yabaContext.Seed();
    }

    app.UseMiddleware<ExceptionMiddleware>();
    app.UseHttpsRedirection();
    app.UseSwagger();
    app.UseSwaggerUI(swaggerOptions => swaggerOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "YabaAPI V1"));

    app.UseRouting();

    app.UseCors();

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}

void ConfigServiceCollection(WebApplicationBuilder builder)
{
    builder.Services
        .Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
    builder.Services
        .Configure<AzureConfig>(builder.Configuration.GetSection("AzureConfig"));
    builder.Services
        .Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));

    builder.Services.AddControllers()
        .AddJsonOptions(opt => opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(swaggerOptions =>
    {
        swaggerOptions.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "YabaAPI", Version = "v1" });
    });

    DependencyResolver.RegisterServices(builder.Services, builder.Configuration);
    
    ConfigureTokenValidation(builder.Services);

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policyBuilder =>
        {
            policyBuilder.AllowAnyHeader();
            policyBuilder.AllowAnyMethod();
            policyBuilder.AllowAnyOrigin();
        });
    });
}

void ConfigureTokenValidation(IServiceCollection services)
{
    var secretKey = builder.Configuration.GetSection("JwtConfig:SecretKey").Value ?? string.Empty;
    var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

    var authConfig = builder.Configuration.GetSection("AuthConfig").Get<AuthConfig>();

    var tokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidIssuers = authConfig?.Issuers,
        ValidateAudience = true,
        ValidAudiences = authConfig?.Audiences,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = securityKey
    };

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = authConfig?.Authority;

            options.RequireHttpsMetadata = builder.Environment.IsDevelopment();
            options.SaveToken = true;
            options.TokenValidationParameters = tokenValidationParameters;
        });
}
