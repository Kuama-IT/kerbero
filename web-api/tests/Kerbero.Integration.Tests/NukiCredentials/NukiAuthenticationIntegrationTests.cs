using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Flurl.Http.Testing;
using Kerbero.Domain.NukiCredentials.Dtos;
using Kerbero.WebApi;
using Kerbero.WebApi.Models.Requests;

namespace Kerbero.Integration.Tests.NukiCredentials;

public class NukiCredentialsIntegrationTests
{
  private readonly KerberoWebApplicationFactory<Program> _application;

  public NukiCredentialsIntegrationTests()
  {
    _application = new KerberoWebApplicationFactory<Program>();
    _application.Server.PreserveExecutionContext = true; // fixture for Flurl
  }

  [Fact]
  public async Task Post_CreateNukiCredentials_WithValidParameters()
  {
    var httpTest = new HttpTest();
    httpTest.RespondWith("everything is good");

    var (client, user) = await _application.CreateUserAndAuthenticateClient();

    var tExpectedDto = new NukiCredentialDto
    {
      Id = 1,
      Token = "VALID_TOKEN"
    };

    var response =
      await client.PostAsJsonAsync(
        "/api/nuki-credentials/", new CreateNukiSmartLockCredentialRequest() { Token = "VALID_TOKEN" });
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var readNukiCredentialDto = await response.Content.ReadFromJsonAsync<NukiCredentialDto>();
    readNukiCredentialDto.Should().NotBeNull();
    readNukiCredentialDto.Should().BeEquivalentTo(tExpectedDto);
  }
  
  [Fact]
  public async Task Post_CreateNukiCredentials_WithInValidParameters()
  {
    var httpTest = new HttpTest();
    httpTest.RespondWith("everything is bad", 500);

    var (client, user) = await _application.CreateUserAndAuthenticateClient();

    var tExpectedDto = new NukiCredentialDto
    {
      Id = 1,
      Token = "VALID_TOKEN"
    };

    var response =
      await client.PostAsJsonAsync(
        "/api/nuki-credentials/", new CreateNukiSmartLockCredentialRequest() { Token = "VALID_TOKEN" });
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }
}