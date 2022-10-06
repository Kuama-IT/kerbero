using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using Flurl.Http.Testing;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Infrastructure.Common.Context;
using Kerbero.Infrastructure.NukiAuthentication.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kerbero.Integration.Tests;

public class NukiAuthenticationIntegrationTest: IDisposable
{
	private readonly WebApplicationFactory<Program> _application;
	private static readonly IConfigurationRoot Config = new ConfigurationBuilder()
		.AddJsonFile("appsettings.Test.json")
		.AddEnvironmentVariables()
		.Build();

	private readonly HttpTest _httpTest;

	public NukiAuthenticationIntegrationTest()
	{
		_application = new KerberoWebApplicationFactory<Program>();
		_application.Server.PreserveExecutionContext = true; // fixture for Flurl
		_httpTest = new HttpTest();
	}
	
	public void Dispose()
	{
		_httpTest.Dispose();
		GC.SuppressFinalize(this);
	}

	[Fact]
	public async Task Program_RedirectForCode_Success_Test()
	{
		var opts = new WebApplicationFactoryClientOptions
		{
			AllowAutoRedirect = false
		};
		var client = _application.CreateClient(opts);

		var clientId = Config["client_id"];
		if (clientId == null) Assert.True(false, "Test cannot find client id value from settings");
		var redirect = await client.GetAsync($"/nuki/auth/start?clientId={clientId}");
		redirect.StatusCode.Should().Be(HttpStatusCode.Found);
		// The url cannot be tested because test client is not connected to internet
	}

	[Fact]
	public async Task Program_RetrieveToken_Success_Test()
	{
		//Arrange
		_httpTest.RespondWithJson(new
		{
			access_token ="ACCESS_TOKEN", 
			token_type = "bearer", 
			expires_in = 2592000, 
			refresh_token = "REFRESH_TOKEN"
		}); 
		
		var client = _application.CreateClient();

		var clientId = Config["client_id"];
		if (clientId == null) Assert.True(false, "Test cannot find client id value from settings");
		var response =
			await client.GetAsync(
				$"/nuki/auth/token/{clientId}?code=eVHvIIXYhytBRA145Bs6GrPXYI4OMPSdN8lS7VeapV4.9EuR0U43Bu" 
					+ $"avL4YAszKxEbGJF1L-OKMLarNwDA8IflU");
		_httpTest.ShouldHaveMadeACall();
		var presentationDto = await response.Content.ReadFromJsonAsync<NukiAccountPresentationDto>();
		presentationDto.Should().NotBeNull();
		presentationDto?.ClientId.Should().Be(clientId);
	}
	
	[Fact]
	public async Task Program_RetrieveToken_InvalidClientCredential_Test()
	{
		_httpTest.RespondWith(status: 401, body: System.Text.Json.JsonSerializer.Serialize(new 
		{
			error_description = "Invalid client credentials.",
			error = "invalid_client"
		})); 
		
		var client = _application.CreateClient();

		var clientId = Config["client_id"];
		if (clientId == null) Assert.True(false, "Test cannot find client id value from settings");
		var response =
			await client.GetAsync(
				$"/nuki/auth/token/{clientId}?code=eVHvIIXYhytBRA145Bs6GrPXYI4OMPSdN8lS7VeapV4.9EuR0U43Bu" 
				+ $"avL4YAszKxEbGJF1L-OKMLarNwDA8IflU");
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		var resJson = await response.Content.ReadFromJsonAsync<JsonObject>();
		resJson.Should().NotBeNull();
		resJson?["message"].Should().NotBeNull();
	}
	
	[Fact]
	public async Task Program_RetrieveToken_NukiTimeout_Test()
	{
		_httpTest.RespondWith(status: (int)HttpStatusCode.RequestTimeout, body: System.Text.Json.JsonSerializer.Serialize(new { })); 
		
		var client = _application.CreateClient();

		var clientId = Config["client_id"];
		if (clientId == null) Assert.True(false, "Test cannot find client id value from settings");
		var response =
			await client.GetAsync(
				$"/nuki/auth/token/{clientId}?code=eVHvIIXYhytBRA145Bs6GrPXYI4OMPSdN8lS7VeapV4.9EuR0U43Bu" 
				+ $"avL4YAszKxEbGJF1L-OKMLarNwDA8IflU");
		response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
		var resJson = await response.Content.ReadFromJsonAsync<JsonObject>();
		resJson.Should().NotBeNull();
		resJson?["message"].Should().NotBeNull();
		
	}	
	
	[Fact]
	public async Task Program_RetrieveToken_UnknownErrorFromNuki_Test()
	{
		_httpTest.RespondWith(status: 435, body: System.Text.Json.JsonSerializer.Serialize(new 
		{
			error_description = "Invalid client credentials.",
			error = "invalid_client"
		})); 
		
		var client = _application.CreateClient();

		var clientId = Config["client_id"];
		if (clientId == null) Assert.True(false, "Test cannot find client id value from settings");
		var response =
			await client.GetAsync(
				$"/nuki/auth/token/{clientId}?code=eVHvIIXYhytBRA145Bs6GrPXYI4OMPSdN8lS7VeapV4.9EuR0U43Bu" 
				+ $"avL4YAszKxEbGJF1L-OKMLarNwDA8IflU");
		response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
		var resJson = await response.Content.ReadFromJsonAsync<JsonObject>();
		resJson.Should().NotBeNull();
		resJson?["message"].Should().NotBeNull();
	}	
	
	private class KerberoWebApplicationFactory<TStartup>
		: WebApplicationFactory<TStartup> where TStartup: class
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
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

			});
		}
	}
}

public class NukiAuthenticationIntegrationInvalidConfigurationTests: IDisposable
{
	private readonly WebApplicationFactory<Program> _application;
	private static readonly IConfigurationRoot Config = new ConfigurationBuilder()
		.AddJsonFile("appsettings.Test.json")
		.AddEnvironmentVariables()
		.Build();

	private readonly HttpTest _httpTest;

	public NukiAuthenticationIntegrationInvalidConfigurationTests()
	{
		_application = new KerberoWebApplicationFactory<Program>();
		_application.Server.PreserveExecutionContext = true; // fixture for Flurl
		_httpTest = new HttpTest();
	}
	
	public void Dispose()
	{
		_httpTest.Dispose();
		GC.SuppressFinalize(this);
	}
	
	[Fact]
	public async Task Program_RedirectForCode_InvalidConfiguration_Test()
	{
		_httpTest.SimulateException(new ArgumentNullException());

		var opts = new WebApplicationFactoryClientOptions
		{
			AllowAutoRedirect = false
		};
		var client = _application.CreateClient(opts);
		
		var clientId = Config["client_id"];
		if (clientId == null) Assert.True(false, "Test cannot find client id value from settings");
		
		
		var redirect = await client.GetAsync($"/nuki/auth/start?clientId={clientId}");

		redirect.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		var resJson = await redirect.Content.ReadFromJsonAsync<JsonObject>();
		resJson.Should().NotBeNull();
		resJson?["message"].Should().NotBeNull();
	}
	
	[Fact]
	public async Task Program_RetrieveToken_InvalidConfiguration_Test()
	{
		var client = _application.CreateClient();

		var clientId = Config["client_id"];
		if (clientId == null) Assert.True(false, "Test cannot find client id value from settings");
		var response =
			await client.GetAsync(
				$"/nuki/auth/token/{clientId}?code=eVHvIIXYhytBRA145Bs6GrPXYI4OMPSdN8lS7VeapV4.9EuR0U43Bu" 
				+ $"avL4YAszKxEbGJF1L-OKMLarNwDA8IflU");
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		var resJson = await response.Content.ReadFromJsonAsync<JsonObject>();
		resJson.Should().NotBeNull();
		resJson?["message"].Should().NotBeNull();
	}

	private class KerberoWebApplicationFactory<TStartup>
		: WebApplicationFactory<TStartup> where TStartup: class
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			
			builder.ConfigureTestServices(services =>
			{
				services.Configure<NukiExternalOptions>(opts =>
				{
					opts.RedirectUriForCode = null!;
				});
			});
			
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
				
			});
		}
	}
}


