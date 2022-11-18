using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using Flurl.Http.Testing;
using Kerbero.Domain.NukiAuthentication.Models.PresentationResponses;
using Kerbero.Infrastructure.Common.Context;
using Kerbero.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kerbero.Integration.Tests.NukiAuthentication;

public class NukiAuthenticationIntegrationTest: IDisposable
{
	private readonly KerberoWebApplicationFactory<Program> _application;

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
	}

	[Fact]
	public async Task Program_RedirectForCode_Success_Test()
	{
		var client = _application.CreateClient();

		var clientId = "CLIENT_ID";
		if (clientId == null) Assert.True(false, "Test cannot find client id value from settings");
		var redirect = await client.GetAsync($"/nuki/auth/start?clientId={clientId}");
		redirect.StatusCode.Should().Be(HttpStatusCode.Found);
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

		var clientId = "CLIENT_ID";
		if (clientId == null) Assert.True(false, "Test cannot find client id value from settings");
		var response =
			await client.GetAsync(
				$"/nuki/auth/token/{clientId}?code=eVHvIIXYhytBRA145Bs6GrPXYI4OMPSdN8lS7VeapV4.9EuR0U43Bu" 
					+ $"avL4YAszKxEbGJF1L-OKMLarNwDA8IflU");
		_httpTest.ShouldHaveMadeACall();
		var presentationDto = await response.Content.ReadFromJsonAsync<NukiAccountPresentationResponse>();
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

		var clientId = "CLIENT_ID";
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

		var clientId = "CLIENT_ID";
		if (clientId == null) Assert.True(false, "Test cannot find client id value from settings");
		var response =
			await client.GetAsync(
				$"/nuki/auth/token/{clientId}?code=eVHvIIXYhytBRA145Bs6GrPXYI4OMPSdN8lS7VeapV4.9EuR0U43Bu" 
				+ $"avL4YAszKxEbGJF1L-OKMLarNwDA8IflU");
		response.StatusCode.Should().Be(HttpStatusCode.BadGateway);
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

		var clientId = "CLIENT_ID";
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
}
