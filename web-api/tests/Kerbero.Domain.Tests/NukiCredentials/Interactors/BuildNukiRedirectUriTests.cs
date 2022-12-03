using System.Web;
using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Repositories;
using Kerbero.Domain.NukiCredentials.Interactors;
using Kerbero.Domain.NukiCredentials.Models;
using Moq;

namespace Kerbero.Domain.Tests.NukiCredentials.Interactors;

public class BuildNukiRedirectUriTests
{
  private readonly BuildNukiRedirectUriInteractor _interactor;
  private readonly Mock<IKerberoConfigurationRepository> _kerberoConfigurationRepositoryMock = new();

  public BuildNukiRedirectUriTests()
  {
    _interactor = new BuildNukiRedirectUriInteractor(_kerberoConfigurationRepositoryMock.Object);
  }

  [Fact]
  async Task It_Creates_A_Correct_Uri()
  {
    var model = new NukiApiConfigurationModel(
      ApiEndpoint: "https://api.nuki.io",
      ClientId: "0",
      Scopes: "account notification",
      ApplicationDomain: "https://test.domain",
      ApplicationRedirectEndpoint: "api/nuki-credentials/confirm-draft"
    );

    _kerberoConfigurationRepositoryMock
      .Setup(repo => repo.GetNukiApiDefinition())
      .ReturnsAsync(() => Result.Ok(model));

    var uriResult = await _interactor.Handle();

    var baseNukiApiEndpoint = "https://api.nuki.io/oauth/authorize";
    var redirectUri = $"redirect_uri={HttpUtility.UrlEncode("https://test.domain/api/nuki-credentials/confirm-draft")}";
    var nukiScopes = $"scope={HttpUtility.UrlEncode("account notification")}";
    var responseTypeAndClient = "response_type=code&client_id=0";
    var expectedUri = $"{baseNukiApiEndpoint}?{responseTypeAndClient}&{nukiScopes}&{redirectUri}";

    uriResult.IsSuccess.Should().BeTrue();
    uriResult.Value.ToString().Should().MatchEquivalentOf(expectedUri);
  }
}