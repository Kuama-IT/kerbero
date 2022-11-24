using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Flurl.Http.Testing;
using Kerbero.WebApi;
using Kerbero.WebApi.Models.Requests;

namespace Kerbero.Integration.Tests.SmartLocks;

public class NukiCloseSmartLockIntegrationTests: IDisposable
{
	private readonly HttpTest _httpTest;
	private readonly KerberoWebApplicationFactory<Program> _application;

	public NukiCloseSmartLockIntegrationTests()
	{
		_application = new KerberoWebApplicationFactory<Program>();
		_application.Server.PreserveExecutionContext = true; // fixture for Flurl
		_httpTest = new HttpTest();
	}

	public void Dispose()
	{
		_httpTest.Dispose();
	}

	[Fact]
	public async Task CloseNukiSmartLock_Success()
	{
		var client = await _application.GetLoggedClient();

		await _application.CreateNukiAccount(IntegrationTestsUtils.GetSeedingNukiAccount());
		await _application.CreateNukiSmartLock(IntegrationTestsUtils.GetSeedingNukiSmartLock());
		
		var response = await client.PutAsJsonAsync("api/smartlocks/lock", new CloseNukiSmartLockRequest(1, 1));

		await response.Content.ReadAsStringAsync();

		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}
	
	[Fact]
	public async Task CloseNukiSmartLock_Error()
	{
		await _application.CreateNukiAccount(IntegrationTestsUtils.GetSeedingNukiAccount());
		await _application.CreateNukiSmartLock(IntegrationTestsUtils.GetSeedingNukiSmartLock());
		var client = await _application.GetLoggedClient();
		_httpTest.RespondWith(status: 401);

		var response = await client.PutAsJsonAsync("api/smartlocks/lock",
			new CloseNukiSmartLockRequest(1, 1));
		var content = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

		response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		Debug.Assert(content is not null);
		Debug.Assert(content["message"] is not null);
		content.Should().NotBeEmpty();
		content["message"].ToString().Should().BeEquivalentTo("The credential are wrong or you can not access to the resource.");
	}
}
