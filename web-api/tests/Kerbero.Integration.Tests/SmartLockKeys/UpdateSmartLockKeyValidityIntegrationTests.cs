using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Kerbero.WebApi;
using Kerbero.WebApi.Dtos.SmartLockKeys;

namespace Kerbero.Integration.Tests.SmartLockKeys;

public class UpdateSmartLockKeyValidityIntegrationTests
{
  private readonly KerberoWebApplicationFactory<Program> _application;

  public UpdateSmartLockKeyValidityIntegrationTests()
  {
    _application = new KerberoWebApplicationFactory<Program>();
    _application.Server.PreserveExecutionContext = true; // fixture for Flurl
  }

  [Fact]
  public async Task UpdateSmartLockKeyBySmartLockKeyId_ValidParameters()
  {
    var (loggedClient, user) = await _application.CreateUserAndAuthenticateClient();

    await _application.CreateNukiCredential(IntegrationTestsUtils.GetNukiCredential(), user.Id);

    var model = await _application.CreateSmartLockKey(IntegrationTestsUtils.GetSmartLockKey());

    var response = await loggedClient.PutAsJsonAsync($"/api/smart-lock-keys/{model.Id}",
      new UpdateSmartLockKeyRequestDto(
        ValidFromDate: model.ValidFrom.AddDays(1),
        ValidUntilDate: model.ValidUntil.AddDays(6)
      )
    );

    response.StatusCode.Should().Be(HttpStatusCode.OK);
    response.IsSuccessStatusCode.Should().BeTrue();
    var smartLockKeyResponse = await response.Content.ReadFromJsonAsync<SmartLockKeyResponseDto>();
    smartLockKeyResponse.Should().NotBeNull();
    smartLockKeyResponse!.ValidFromDate.Should().BeCloseTo(model.ValidFrom.AddDays(1), TimeSpan.FromSeconds(5));
    smartLockKeyResponse.ValidUntilDate.Should().BeCloseTo(model.ValidUntil.AddDays(6), TimeSpan.FromSeconds(5));
  }
}