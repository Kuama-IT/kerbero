using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using Flurl.Http.Testing;
using Kerbero.Domain.NukiActions.Models.PresentationResponse;
using Kerbero.WebApi;
using Kerbero.WebApi.Models.Requests;

namespace Kerbero.Integration.Tests.SmartLocks;

public class NukiCreateSmartLockIntegrationTests: IDisposable
{
	private readonly KerberoWebApplicationFactory<Program> _application;

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
		
		var client = await _application.GetLoggedClient();
		
		var response = await client.PostAsJsonAsync("api/smartlocks/import/nuki/", new CreateNukiSmartLockRequest(1 ,1));

		response.Should().BeSuccessful();
		var content = await response.Content.ReadFromJsonAsync<KerberoSmartLockPresentationResponse>();
		content!.Should().BeEquivalentTo(new KerberoSmartLockPresentationResponse
		{
			ExternalName = "string",
			ExternalType = 0,
			AccountId = 1,
			ExternalSmartLockId = 0,
			SmartLockId = content!.SmartLockId
		});
	}

	[Fact]
	public async Task CreateNukiSmartLocks_Error_Test()
	{
		await _application.CreateNukiAccount(IntegrationTestsUtils.GetSeedingNukiAccount());

		_httpTest.RespondWith(status: 401, body: JsonSerializer.Serialize(new 
		{
			error_description = "Invalid client credentials.",
			error = "invalid_client"
		})); 
		var client = await _application.GetLoggedClient();

		// Act
		var response = await client.PostAsJsonAsync("api/smartlocks/import/nuki/", new CreateNukiSmartLockRequest(1 ,1));

		response.IsSuccessStatusCode.Should().BeFalse();
		var content = await response.Content.ReadFromJsonAsync<JsonObject>();
		content!["message"]?.ToString().Should().BeEquivalentTo("There are missing or wrong parameter in the request: invalid_client: Invalid client credentials..");
	}
}
