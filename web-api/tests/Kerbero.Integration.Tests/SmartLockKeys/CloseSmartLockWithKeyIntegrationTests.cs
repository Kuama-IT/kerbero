using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Flurl.Http.Testing;
using Kerbero.WebApi;
using Kerbero.WebApi.Dtos.SmartLockKeys;

namespace Kerbero.Integration.Tests.SmartLockKeys;

public class CloseSmartLockWithKeyIntegrationTests : IDisposable
{
  private readonly KerberoWebApplicationFactory<Program> _application;
  private readonly HttpTest _httpTest;

  public CloseSmartLockWithKeyIntegrationTests()
  {
    _application = new KerberoWebApplicationFactory<Program>();
    _httpTest = new HttpTest();
  }

  public void Dispose()
  {
    _httpTest.Dispose();
  }

  [Fact]
  public async Task CloseSmartLockWithKey_WithAValidRequest()
  {
    var (_, user) = await _application.CreateUserAndAuthenticateClient();
    await _application.CreateNukiCredential(IntegrationTestsUtils.GetNukiCredential(), user.Id);
    var tSmartLockKey = await _application.CreateSmartLockKey(IntegrationTestsUtils.GetSmartLockKey());

    var tRequest = new OpenSmartLockWithKeyRequestDto(tSmartLockKey.Id, tSmartLockKey.Password);
    var httpClient = _application.CreateClient(); // not authenticated client
    var httpResponse = await httpClient.PutAsJsonAsync("api/smart-lock-keys/close-smart-lock", tRequest);

    httpResponse.IsSuccessStatusCode.Should().BeTrue();
  }

  [Fact]
  public async Task CloseSmartLockWithKey_WithAInvalidRequest()
  {
    var (_, user) = await _application.CreateUserAndAuthenticateClient();
    await _application.CreateNukiCredential(IntegrationTestsUtils.GetNukiCredential(), user.Id);
    var tSmartLockKey = await _application.CreateSmartLockKey(IntegrationTestsUtils.GetFutureSmartLockKey());

    var tRequest = new OpenSmartLockWithKeyRequestDto(tSmartLockKey.Id, tSmartLockKey.Password);
    var httpClient = _application.CreateClient(); // not authenticated client
    var httpResponse = await httpClient.PutAsJsonAsync("api/smart-lock-keys/close-smart-lock", tRequest);

    httpResponse.IsSuccessStatusCode.Should().BeFalse();
    httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }
}