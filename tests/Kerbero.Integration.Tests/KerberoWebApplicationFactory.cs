using Kerbero.Infrastructure.Common.Context;
using Kerbero.Infrastructure.Common.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kerbero.Integration.Tests;

public class KerberoWebApplicationFactory<TStartup>
	: WebApplicationFactory<TStartup> where TStartup: class
{
	private readonly bool _badConfig;

	public readonly IConfigurationRoot Config = new ConfigurationBuilder()
		.AddJsonFile("appsettings.Test.json")
		.AddEnvironmentVariables()
		.Build();

	public KerberoWebApplicationFactory(bool badConfig = false)
	{
		_badConfig = badConfig;
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		if (_badConfig)
		{
			builder.ConfigureTestServices(services =>
			{
				services.Configure<NukiExternalOptions>(opts =>
				{
					opts.RedirectUriForCode = null!;
				});
			});
		}
		
		builder.ConfigureServices(services =>
		{
			var descriptor = services.SingleOrDefault(
				d => d.ServiceType ==
				     typeof(DbContextOptions<ApplicationDbContext>));

			if (descriptor is not null)
				services.Remove(descriptor);

			services.AddDbContext<ApplicationDbContext>(options => 
				options.UseInMemoryDatabase(Config["ConnectionStrings:TestString"]!));



			var sp = services.BuildServiceProvider();

			using var scope = sp.CreateScope();
			var scopedServices = scope.ServiceProvider;
			var db = scopedServices.GetRequiredService<ApplicationDbContext>();
			var logger = scopedServices
				.GetRequiredService<ILogger<KerberoWebApplicationFactory<TStartup>>>();

			db.Database.EnsureCreated();

			try
			{
				IntegrationTestsUtils.InitializeDbForTests(db);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "An error occurred seeding the " +
				                    "database with test messages. Error: {Message}", ex.Message);
			}
				
		});
	}
}