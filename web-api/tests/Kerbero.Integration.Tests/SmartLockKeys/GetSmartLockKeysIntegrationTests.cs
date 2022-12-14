using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Flurl.Http.Testing;
using Kerbero.WebApi;
using Kerbero.WebApi.Dtos.SmartLockKeys;

namespace Kerbero.Integration.Tests.SmartLockKeys;

public class GetSmartLockKeysIntegrationTests : IDisposable
{
  private readonly KerberoWebApplicationFactory<Program> _application;
  private readonly HttpTest _httpTest;

  public GetSmartLockKeysIntegrationTests()
  {
    _application = new KerberoWebApplicationFactory<Program>();
    _httpTest = new HttpTest();
  }

  public void Dispose()
  {
    _httpTest.Dispose();
  }

  [Fact]
  public async Task GetSmartLockKeys_ValidSession()
  {
    var rawResponse = await File.ReadAllTextAsync("JsonData/get-nuki-smartlock-response.json");
    _httpTest.RespondWith(rawResponse);

    var (loggedClient, user) = await _application.CreateUserAndAuthenticateClient();
    await _application.CreateNukiCredential(IntegrationTestsUtils.GetNukiCredential(), user.Id);
    var tSmartLockKey = await _application.CreateSmartLockKey(IntegrationTestsUtils.GetSmartLockKey());

    var httpResponseMessage = await loggedClient.GetAsync("api/smart-lock-keys");

    httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
    httpResponseMessage.IsSuccessStatusCode.Should().BeTrue();
    var smartLockKeyDtos = await httpResponseMessage.Content.ReadFromJsonAsync<SmartLockKeyListResponseDto>();
    var tExpected = new SmartLockKeyResponseDto()
    {
      Password = tSmartLockKey.Password,
      ValidFromDate = tSmartLockKey.ValidFrom,
      Id = tSmartLockKey.Id,
      ValidUntilDate = tSmartLockKey.ValidUntil,
    };
    smartLockKeyDtos!.SmartLockKeys.Should().BeEquivalentTo(new List<SmartLockKeyResponseDto>()
    {
      tExpected
    });
  }
}