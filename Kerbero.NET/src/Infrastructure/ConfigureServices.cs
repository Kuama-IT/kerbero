using Kerbero.NET.Application.Common.Interfaces;
using Kerbero.NET.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = GetDbConnectionString(configuration);
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString,
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
    
    private static string GetDbConnectionString(IConfiguration configuration)
    {
        var dbSettings = configuration.GetSection("ConnectionStrings");
        var connectionString = new SqlConnectionStringBuilder
        {
            ["Server"] = dbSettings.GetValue<string>("Server"),
            ["Database"] = dbSettings.GetValue<string>("DbName"),
            ["User"] = dbSettings.GetValue<string>("User"),
            ["Password"] = dbSettings.GetValue<string>("Password"),
            TrustServerCertificate = true
        };
        return connectionString.ToString();
    }
}
