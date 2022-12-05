using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Flurl.Http.Testing;
using Kerbero.WebApi;
using Kerbero.WebApi.Dtos.NukiCredentials;

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

    var (client, _) = await _application.CreateUserAndAuthenticateClient();
    
    httpTest
      .ForCallsTo("https://api.nuki.io/account")
      .RespondWithJson(new {accountId = 0, email = "nuki@test.com", name = "Tester"});

    var tExpectedDto = new NukiCredentialResponseDto
    {
      Id = 1,
      Token = "VALID_TOKEN"
    };

    var response =
      await client.PostAsJsonAsync(
        "/api/nuki-credentials/", new CreateNukiCredentialRequestDto() { Token = "VALID_TOKEN" });
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var readNukiCredentialDto = await response.Content.ReadFromJsonAsync<NukiCredentialResponseDto>();
    readNukiCredentialDto.Should().NotBeNull();
    readNukiCredentialDto.Should().BeEquivalentTo(tExpectedDto);
  }
  
  [Fact]
  public async Task Post_CreateNukiCredentials_WithInValidParameters()
  {
    var httpTest = new HttpTest();
    httpTest.RespondWith("everything is bad", 500);

    var (client, _) = await _application.CreateUserAndAuthenticateClient();

    var response =
      await client.PostAsJsonAsync(
        "/api/nuki-credentials/", new CreateNukiCredentialRequestDto() { Token = "VALID_TOKEN" });
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  [Fact]
  public async Task Delete_DeleteNukiCredentials_WithValidParameters()
  {
    var (loggedClient, user) = await _application.CreateUserAndAuthenticateClient();
		
    var tNukiCredential = await _application.CreateNukiCredential(IntegrationTestsUtils.GetNukiCredential(), user.Id);

    var httpResponseMessage = await loggedClient.DeleteAsync($"api/nuki-credentials/{tNukiCredential.Id}");
    httpResponseMessage.IsSuccessStatusCode.Should().BeTrue();

    var nukiCredentialResponseDto = await httpResponseMessage.Content.ReadFromJsonAsync<NukiCredentialResponseDto>();
    
    var getResponseMessage = await loggedClient.GetAsync($"api/nuki-credentials/");
    var nukiCredentialsResponseDtos = await getResponseMessage.Content.ReadFromJsonAsync<List<NukiCredentialResponseDto>>();

    nukiCredentialsResponseDtos.Should().NotContain(nukiCredentialResponseDto!);
  }
}
