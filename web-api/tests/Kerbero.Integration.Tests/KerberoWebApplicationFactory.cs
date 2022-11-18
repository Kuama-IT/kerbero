using DotNetEnv;
using Kerbero.Domain.NukiActions.Entities;
using Kerbero.Domain.NukiActions.Repositories;
using Kerbero.Domain.NukiAuthentication.Entities;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Kerbero.Infrastructure.Common.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kerbero.Integration.Tests;

public class KerberoWebApplicationFactory<TStartup>
	: WebApplicationFactory<TStartup> where TStartup: class
{
	private readonly bool _badConfig;

	public readonly IConfigurationRoot Config = new ConfigurationBuilder()
		.AddEnvironmentVariables(_ => Env.TraversePath().Load())
		.Build();

	public KerberoWebApplicationFactory(bool badConfig = false)
	{
		_badConfig = badConfig;
		ClientOptions.AllowAutoRedirect = false;
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		if (_badConfig)
		{
			builder.ConfigureTestServices(services =>
			{
				// services.Co
			});
		}

		builder.UseConfiguration(Config);

		builder.ConfigureServices(services =>
		{
			var descriptor = services.SingleOrDefault(
				d => d.ServiceType ==
				     typeof(DbContextOptions<ApplicationDbContext>));

			if (descriptor is not null)
				services.Remove(descriptor);

			services.AddDbContext<ApplicationDbContext>(options => 
				options.UseInMemoryDatabase(Config["INMEMORY_CONNECTION_STRING"]!));

			var sp = services.BuildServiceProvider();

			using var scope = sp.CreateScope();
			var scopedServices = scope.ServiceProvider;
			var db = scopedServices.GetRequiredService<ApplicationDbContext>();

			db.Database.EnsureCreated();
		});
	}

	public async Task CreateNukiAccount(NukiAccount account)
	{
		using var scope = Services.CreateScope();
		var accountPersistentRepository = scope.ServiceProvider.GetRequiredService<INukiAccountPersistentRepository>();
		await accountPersistentRepository.Create(account);
	}

	public async Task CreateNukiSmartLock(NukiSmartLock smartLock)
	{
		using var scope = Services.CreateScope();
		var accountPersistentRepository = scope.ServiceProvider.GetRequiredService<INukiSmartLockPersistentRepository>();
		await accountPersistentRepository.Create(smartLock);
	}
}
