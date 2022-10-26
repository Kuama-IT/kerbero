using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using Flurl.Http.Testing;
using Kerbero.Infrastructure.Common.Context;
using Kerbero.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kerbero.Integration.Tests;

public class NukiSmartLocksListIntegrationTests: IDisposable
{
	private readonly HttpTest _httpTest;
	private readonly HttpClient _client;
	private readonly KerberoWebApplicationFactory<Program> _application;
	private readonly object _nukiJsonSmartLockResponse;

	public NukiSmartLocksListIntegrationTests()
	{
		_application = new KerberoWebApplicationFactory<Program>();
		_application.Server.PreserveExecutionContext = true; // fixture for Flurl
		_httpTest = new HttpTest();
		_client = _application.CreateClient();
		var json = File.ReadAllText("JsonData/get-nuki-smartlock-response.json");
		_nukiJsonSmartLockResponse = JsonSerializer.Deserialize<dynamic>(json) ?? throw new InvalidOperationException();
	}
	
	public void Dispose()
	{
		_httpTest.Dispose();
	}

	[Fact]
	public async Task GetSmartLocksListByKerberoAccount_Success()
	{
		// Arrange
		await _application.CreateNukiAccount(IntegrationTestsUtils.GetSeedingNukiAccount());
		_httpTest.RespondWithJson(
			new[]
			{
				_nukiJsonSmartLockResponse
			});
		
		var response = await _client.GetAsync("api/nuki/smartlock?accountId=1");

		var content = await response.Content.ReadAsStringAsync();
		response.EnsureSuccessStatusCode();
		content.Should()
			.BeEquivalentTo(
			"[{\"externalName\":\"string\",\"externalType\":0,\"accountId\":0,\"externalSmartLockId\":0,\"smartLockId\":0}]");
	}
	
	[Fact]
	public async Task GetSmartLocksListByKerberoAccount_RefreshToken_AndCannotParseTheResponse_Text()
	{
		await _application.CreateNukiAccount(IntegrationTestsUtils.GetSeedingNukiAccount());
		_httpTest.RespondWithJson(new
		{
			access_token ="ACCESS_TOKEN", 
			token_type = "bearer", 
			expires_in = 2592000, 
			refresh_token = "REFRESH_TOKEN"
		}); // from nuki documentation
		
		var response = await _client.GetAsync("api/nuki/smartlock?accountId=1");

		response.IsSuccessStatusCode.Should().BeFalse();
	}
	
	[Fact]
	public async Task GetSmartLocksListByKerberoAccount_Unauthorized_Test()
	{
		await _application.CreateNukiAccount(IntegrationTestsUtils.GetSeedingNukiAccount());
		_httpTest.RespondWith(status: 401, body: JsonSerializer.Serialize(new
		{
			detailMessage = "Your access token is not authorized",
			stackTrace = Array.Empty<object>(),
			suppressedExceptions = Array.Empty<object>()
		})); 
		
		var response = await _client.GetAsync("api/nuki/smartlock?accountId=1");

		response.IsSuccessStatusCode.Should().BeFalse();
		var resJson = await response.Content.ReadFromJsonAsync<JsonObject>();
		resJson.Should().NotBeNull();
		resJson?["message"].Should().NotBeNull();
	}
	
	[Fact]
	public async Task GetSmartLocksListByKerberoAccount_NoAccount_Test()
	{
		await _application.CreateNukiAccount(IntegrationTestsUtils.GetSeedingNukiAccount());
		var response = await _client.GetAsync("api/nuki/smartlock?accountId=0");

		response.IsSuccessStatusCode.Should().BeFalse();
		response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		var resJson = await response.Content.ReadFromJsonAsync<JsonObject>();
		resJson.Should().NotBeNull();
		resJson?["message"].Should().NotBeNull();
	}
	
	[Fact]
	public async Task GetSmartLocksListByKerberoAccount_TimeoutNuki_Test()
	{
		await _application.CreateNukiAccount(IntegrationTestsUtils.GetSeedingNukiAccount());
		_httpTest.RespondWith(status: 408); 
		
		var response = await _client.GetAsync("api/nuki/smartlock?accountId=1");

		response.IsSuccessStatusCode.Should().BeFalse();
		response.StatusCode.Should().Be(HttpStatusCode.BadGateway);
		var resJson = await response.Content.ReadFromJsonAsync<JsonObject>();
		resJson.Should().NotBeNull();
		resJson?["message"].Should().NotBeNull();
	}
}
