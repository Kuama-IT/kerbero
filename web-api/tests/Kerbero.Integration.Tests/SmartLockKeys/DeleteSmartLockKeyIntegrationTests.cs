using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Flurl.Http.Testing;
using Kerbero.WebApi;
using Kerbero.WebApi.Dtos.SmartLockKeys;

namespace Kerbero.Integration.Tests.SmartLockKeys;

public class DeleteSmartLockKeyIntegrationTests : IDisposable
{
  private readonly KerberoWebApplicationFactory<Program> _application;
  private readonly HttpTest _httpTest;

  public DeleteSmartLockKeyIntegrationTests()
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
  public async Task DeleteSmartLockKeys_ValidId()
  {
    var rawResponse = await File.ReadAllTextAsync("JsonData/get-nuki-smartlock-response.json");
    _httpTest.RespondWith(rawResponse);

    var (loggedClient, user) = await _application.CreateUserAndAuthenticateClient();
    await _application.CreateNukiCredential(IntegrationTestsUtils.GetNukiCredential(), user.Id);
    var tSmartLockKey = await _application.CreateSmartLockKey(IntegrationTestsUtils.GetSmartLockKey());

    var httpResponseMessage = await loggedClient.DeleteAsync($"api/smart-lock-keys/{tSmartLockKey.Id}");

    httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
    httpResponseMessage.IsSuccessStatusCode.Should().BeTrue();
    var smartLockKeyDto = await httpResponseMessage.Content.ReadFromJsonAsync<SmartLockKeyResponseDto>();
    var tExpected = new SmartLockKeyResponseDto()
    {
      Password = tSmartLockKey.Password,
      ValidFromDate = tSmartLockKey.ValidFrom,
      Id = tSmartLockKey.Id,
      ValidUntilDate = tSmartLockKey.ValidUntil,
    };
    smartLockKeyDto.Should().BeEquivalentTo(tExpected);

    var getResponseMessage = await loggedClient.GetAsync($"api/smart-lock-keys/");
    var smartLockKeysResponseDtos = await getResponseMessage.Content.ReadFromJsonAsync<List<SmartLockKeyResponseDto>>();
    smartLockKeysResponseDtos.Should().NotContain(smartLockKeyDto!);
  }
}