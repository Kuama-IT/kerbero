using DotNetEnv;
using System.Net.Http.Json;
using Kerbero.Domain.NukiActions.Entities;
using Kerbero.Domain.NukiActions.Repositories;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Kerbero.Identity.Library.Modules.Authentication.Dtos;
using Kerbero.Identity.Modules.Users.Entities;
using Kerbero.Identity.Modules.Users.Services;
using Kerbero.Infrastructure.Common.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kerbero.Integration.Tests;

public class KerberoWebApplicationFactory<TStartup>
  : WebApplicationFactory<TStartup> where TStartup : class
{
  private readonly IConfigurationRoot _config = new ConfigurationBuilder()
    .AddEnvironmentVariables(_ => Env.TraversePath().Load(".env-test"))
    .Build();

  private readonly SqliteConnection _connection;

  public KerberoWebApplicationFactory()
  {
    _connection = new SqliteConnection(_config["SQLITE_CONNECTION_STRING"]);
    _connection.Open();
    ClientOptions.AllowAutoRedirect = false;
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.UseConfiguration(_config);

    builder.ConfigureServices(services =>
    {
      var descriptor = services.SingleOrDefault(
        d => d.ServiceType ==
             typeof(DbContextOptions<ApplicationDbContext>));

      if (descriptor is not null)
        services.Remove(descriptor);

      services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(_connection));

      var sp = services.BuildServiceProvider();

      using var scope = sp.CreateScope();
      var scopedServices = scope.ServiceProvider;
      var db = scopedServices.GetRequiredService<ApplicationDbContext>();

      db.Database.EnsureCreated();
    });
  }

  public async Task<HttpClient> CreateUserAndAuthenticateClient()
  {
    ClientOptions.HandleCookies = true;
    ClientOptions.BaseAddress = new Uri("https://localhost");
    var client = CreateClient();
    
    await CreateUser(IntegrationTestsUtils.GetSeedingUser());

    await client.PostAsJsonAsync("api/authentication/login", new LoginDto
    {
      Email = "test@test.com",
      Password = "Test.0"
    });
    return client;
  }

  public async Task CreateNukiCredential(NukiCredential model, Guid userId)
  {
    using var scope = Services.CreateScope();
    var accountPersistentRepository = scope.ServiceProvider.GetRequiredService<INukiCredentialRepository>();
    await accountPersistentRepository.Create(model, userId);
  }

  private async Task CreateUser(User user)
  {
    using var scope = Services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<IUserManager>();
    await userManager.Create(user, "Test.0");
  }
}