using KerberoWebApi.Clients.Nuki;
using KerberoWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.

// Main Domain where the app "will live"
var mainDomain = builder.Configuration.GetValue<string>("MainDomain");

// Load Nuki Options
var nukiConfigurationOptions = builder.Configuration.GetSection("NukiOptions");
// TODO throw if null

var nukiOptions = new NukiVendorClientOptions(
  clientSecret: nukiConfigurationOptions.GetValue<string>("ClientSecret"),
  redirectUriForCode: nukiConfigurationOptions.GetValue<string>("RedirectUriForCode"),
  redirectUriForAuthToken: nukiConfigurationOptions.GetValue<string>("RedirectUriForAuthToken"),
  scopes: nukiConfigurationOptions.GetValue<string>("Scopes"),
  baseUrl: nukiConfigurationOptions.GetValue<string>("BaseUrl")
);

builder.Services.AddScoped(provider => new NukiVendorClient(nukiOptions));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Needed for session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
  options.IdleTimeout = TimeSpan.FromSeconds(10);
  options.Cookie.HttpOnly = true;
  options.Cookie.IsEssential = true;
});

var app = builder.Build();

// We need to be on ssl in order to talk to Nuki Apis
app.UseHttpsRedirection();

app.UseSession();

// app.UseAuthorization();
app.MapControllers();

app.Run();