using System.Data.Common;
using System.Text.Json.Serialization;
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

        #region vendor clients options

        // Load Nuki Options
        var nukiConfigurationOptions = builder.Configuration.GetSection("NukiOptions")
          ?? throw new SystemException("Unable to load Nuki Options from app settings");

        var nukiOptions = new NukiVendorClientOptions(
          // To uncomment after getting the secret
          mainDomain: nukiConfigurationOptions.GetValue<string>("MainDomain"),
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
        builder.Services.AddScoped( _ => new NukiClientAuthentication(nukiOptions));

        // client implementations
        // binding the interface IVendorClient to the client implementation should create the list of client for each controller
        builder.Services.AddScoped<IVendorClient, NukiClient>( _ => new NukiClient(nukiOptions));

        #endregion

        builder.Services.AddControllers().AddJsonOptions(x =>
	        // option that ignore nested class cycle when returning an HttpResponse from controller
	        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); 

        var app = builder.Build();

        // We need to be on ssl in order to talk to Nuki Apis
        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
    
    /// <summary>
    /// Build the connection string for the application DB
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    private static string GetDbConnectionString(ref WebApplicationBuilder builder)
    {
        var dbSettings = builder.Configuration.GetSection("DbSettings");
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
