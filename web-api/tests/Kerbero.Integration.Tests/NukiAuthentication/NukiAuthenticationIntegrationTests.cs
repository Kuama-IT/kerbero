using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Kerbero.Domain.NukiAuthentication.Dtos;
using Kerbero.WebApi;
using Kerbero.WebApi.Models.Requests;

namespace Kerbero.Integration.Tests.NukiAuthentication;

public class NukiAccountsTests
{
  private readonly KerberoWebApplicationFactory<Program> _application;

  public NukiAccountsTests()
  {
    _application = new KerberoWebApplicationFactory<Program>();
    _application.Server.PreserveExecutionContext = true; // fixture for Flurl
  }

  [Fact]
  public async Task Post_CreateNukiCredentials_WithValidParameters()
  {
    var client = await _application.CreateUserAndAuthenticateClient();

    var tExpectedDto = new NukiCredentialDto
    {
      Id = 1,
      Token = "VALID_TOKEN"
    };

    var response =
      await client.PostAsJsonAsync(
        "/api/nukicredentials/", new CreateNukiSmartLockCredentialRequest() { Token = "VALID_TOKEN" });
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var readNukiCredentialDto = await response.Content.ReadFromJsonAsync<NukiCredentialDto>();
    readNukiCredentialDto.Should().NotBeNull();
    readNukiCredentialDto.Should().BeEquivalentTo(tExpectedDto);
  }
}