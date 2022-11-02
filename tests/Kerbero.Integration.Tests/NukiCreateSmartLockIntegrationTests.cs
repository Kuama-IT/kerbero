using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using Flurl.Http.Testing;
using Kerbero.Domain.NukiActions.Models.PresentationResponse;
using Kerbero.Infrastructure.Common.Context;
using Kerbero.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kerbero.Integration.Tests;

public class NukiCreateSmartLockIntegrationTests: IDisposable
{
	private readonly KerberoWebApplicationFactory<Program> _application;
	private static readonly IConfigurationRoot Config = new ConfigurationBuilder()
		.AddJsonFile("appsettings.Test.json")
		.AddEnvironmentVariables()
		.Build();

	private readonly HttpTest _httpTest;
	private readonly object _nukiJsonSmartLockResponse;

	public NukiCreateSmartLockIntegrationTests()
	{
		_application = new KerberoWebApplicationFactory<Program>();
		_application.Server.PreserveExecutionContext = true; // fixture for Flurl
		_httpTest = new HttpTest();
		var json = File.ReadAllText("JsonData/get-nuki-smartlock-response.json");
		_nukiJsonSmartLockResponse = JsonSerializer.Deserialize<dynamic>(json) ?? throw new InvalidOperationException();
	}
	
	public void Dispose()
	{
		_httpTest.Dispose();
	}

	[Fact]
	public async Task CreateNukiSmartLocks_Success_Test()
	{
		await _application.CreateNukiAccount(IntegrationTestsUtils.GetSeedingNukiAccount());

		_httpTest.RespondWithJson(_nukiJsonSmartLockResponse);
		
		var client = _application.CreateClient();

		var response = await client.PostAsync($"api/nuki/smartlock/{1}?accountId=1", null);

		response.EnsureSuccessStatusCode();
		var content = await response.Content.ReadFromJsonAsync<KerberoSmartLockPresentationResponse>();
		content!.Should().BeEquivalentTo(new KerberoSmartLockPresentationResponse
		{
			ExternalName = "string",
			ExternalType = 0,
			AccountId = 1,
			ExternalSmartLockId = 0,
			SmartLockId = 1
		});
	}

	[Fact]
	public async Task CreateNukiSmartLocks_Error_Test()
	{
		await _application.CreateNukiAccount(IntegrationTestsUtils.GetSeedingNukiAccount());

		_httpTest.RespondWith(status: 401, body: System.Text.Json.JsonSerializer.Serialize(new 
		{
			error_description = "Invalid client credentials.",
			error = "invalid_client"
		})); 
		var client = _application.CreateClient();

		// Act
		var response = await client.PostAsync($"api/nuki/smartlock/{1}?accountId=1", null);

		response.IsSuccessStatusCode.Should().BeFalse();
		var content = await response.Content.ReadFromJsonAsync<JsonObject>();
		content!["message"]?.ToString().Should().BeEquivalentTo("There are missing or wrong parameter in the request: invalid_client: Invalid client credentials..");
	}
}