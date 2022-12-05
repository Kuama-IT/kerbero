using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Flurl.Http.Testing;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.WebApi;
using Kerbero.WebApi.Dtos.SmartLocks;

namespace Kerbero.Integration.Tests.SmartLocks;

public class SmartLockIntegrationTests
{
  private readonly KerberoWebApplicationFactory<Program> _application;
  private readonly HttpTest _httpTest;

  public SmartLockIntegrationTests()
  {
    _httpTest = new HttpTest();
    _application = new KerberoWebApplicationFactory<Program>();
    _application.Server.PreserveExecutionContext = true; // fixture for Flurl
  }

  [Fact]
  public async Task PUT_OpenSmartLock_WithValidParameters()
  {
    _httpTest
      .ForCallsTo("https://api.nuki.io/account")
      .RespondWith("OK", 200);

    _httpTest
      .ForCallsTo("https://api.nuki.io/smartlock/0/unlock")
      .RespondWith("OK", 204);

    var (client, user) = await _application.CreateUserAndAuthenticateClient();

    var credentials = await _application.CreateNukiCredential(new NukiCredentialModel
    {
      Id = 0,
      Token = "A_TOKEN",
      NukiEmail = "test@nuki.com"
    }, userId: user.Id);

    var response =
      await client.PutAsJsonAsync(
        "/api/smart-locks/0/open", new OpenSmartLockRequestDto(
          CredentialsId: credentials.Id,
          SmartLockProvider: "nuki"
        )
      );
    response.StatusCode.Should().Be(HttpStatusCode.NoContent);
  }

  [Fact]
  public async Task PUT_OpenSmartLock_WithInvalidParameters()
  {
    _httpTest
      .ForCallsTo("https://api.nuki.io/account")
      .RespondWith("OK", 200);

    var (client, user) = await _application.CreateUserAndAuthenticateClient();

    var response =
      await client.PutAsJsonAsync(
        "/api/smart-locks/0/open", new OpenSmartLockRequestDto(
          CredentialsId: 0, // these credentials do not belong to the user
          SmartLockProvider: "nuki"
        )
      );
    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }
}