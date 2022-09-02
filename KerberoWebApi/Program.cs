using System.Data.Common;
using KerberoWebApi.Clients;
using KerberoWebApi.Clients.Nuki;
using KerberoWebApi.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();


        #region register db context

        var connectionString = GetDbConnectionString(ref builder);
        builder.Services.AddDbContext<ApplicationContext>(opt =>
          opt.UseSqlServer(connectionString));        

        #endregion

        // Add services to the container.
        #region load parameter from settings

        // Main Domain where the app "will live" TODO usarlo
        var mainDomain = builder.Configuration.GetValue<string>("MainDomain");

        #endregion

        #region vendor clients options

        // Load Nuki Options
        var nukiConfigurationOptions = builder.Configuration.GetSection("NukiOptions")
          ?? throw new SystemException("Unable to load Nuki Options from app settings");

        var nukiOptions = new NukiVendorClientOptions(
          // To uncomment after getting the secret
          clientSecret: nukiConfigurationOptions.GetValue<string>("ClientSecret"),
          redirectUriForCode: nukiConfigurationOptions.GetValue<string>("RedirectUriForCode"),
          redirectUriForAuthToken: nukiConfigurationOptions.GetValue<string>("RedirectUriForAuthToken"),
          scopes: nukiConfigurationOptions.GetValue<string>("Scopes"),
          baseUrl: nukiConfigurationOptions.GetValue<string>("BaseUrl")
        );

        #endregion

        #region vendor authentication services
        // add here the authentication services, but first you must check vendor clients options.
        // authentication services
        builder.Services.AddScoped<NukiClientAuthentication>(provider => new NukiClientAuthentication(nukiOptions));

        // client implementations
        // binding the interface IVendorClient to the client implementation should create the list of client for each controller
        builder.Services.AddScoped<IVendorClient, NukiClient>(provider => new NukiClient(nukiOptions));

        #endregion

        builder.Services.AddControllers();
        // builder.Services.AddEndpointsApiExplorer();

        var app = builder.Build();

        // We need to be on ssl in order to talk to Nuki Apis
        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
    
    private static string GetDbConnectionString(ref WebApplicationBuilder builder)
    {
        var dbSettings = builder.Configuration.GetSection("DbSettings");
        var connectionString = new SqlConnectionStringBuilder();
        connectionString["Server"] = dbSettings.GetValue<string>("Server");
        connectionString["Database"] = dbSettings.GetValue<string>("DbName");
        connectionString["User"] = dbSettings.GetValue<string>("User");
        connectionString["Password"] = dbSettings.GetValue<string>("Password");
        connectionString.TrustServerCertificate = true;
        return connectionString.ToString();
    }

}
