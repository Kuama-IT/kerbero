using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Repositories;
using Kerbero.Domain.NukiCredentials.Interactors;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.NukiCredentials.Repositories;
using Moq;

namespace Kerbero.Domain.Tests.NukiCredentials.Interactors;

public class ConfirmNukiDraftCredentialsInteractorTests
{
  private readonly ConfirmNukiDraftCredentialsInteractor _interactor;
  private readonly Mock<IKerberoConfigurationRepository> _kerberoConfigurationRepositoryMock = new();
  private readonly Mock<INukiCredentialRepository> _nukiCredentialRepositoryMock = new();

  public ConfirmNukiDraftCredentialsInteractorTests()
  {
    _interactor = new ConfirmNukiDraftCredentialsInteractor(
      kerberoConfigurationRepository: _kerberoConfigurationRepositoryMock.Object,
      nukiCredentialRepository: _nukiCredentialRepositoryMock.Object
    );
  }

  [Fact]
  async Task It_Confirms_A_Draft_Credential()
  {
    var userId = Guid.NewGuid();
    var token = "A_TOKEN";
    var configurationModel = new NukiApiConfigurationModel(
      ApiEndpoint: "https://api.nuki.io",
      ClientId: "0",
      Scopes: "account notification",
      ApplicationDomain: "https://test.domain",
      ApplicationRedirectEndpoint: "api/nuki-credentials/confirm-draft",
      WebAppSuccessRedirectEndpoint: "user",
      WebAppFailureRedirectEndpoint: "user/nuki-fail",
      WebAppDomain: "https://test.webapp"
    );

    var finalModel = new NukiCredentialModel()
    {
      Id = 0,
      Token = token,
      UserId = userId,
      NukiEmail = "test@nuki.com"
    };

    var refreshModel = new NukiRefreshableCredentialModel(
      Token: token,
      RefreshToken: token,
      TokenExpiresIn: 27900,
      Error: null,
      CreatedAt: DateTime.Now
    );

    var draftModel = new NukiCredentialDraftModel(
      UserId: userId,
      Id: 0
    );

    _kerberoConfigurationRepositoryMock
      .Setup(repo => repo.GetNukiApiDefinition())
      .ReturnsAsync(() => Result.Ok(configurationModel));

    _nukiCredentialRepositoryMock
      .Setup(repo => repo.GetDraftCredentialsByUserId(It.IsAny<Guid>()))
      .ReturnsAsync(Result.Ok(draftModel));

    _nukiCredentialRepositoryMock
      .Setup(repo => repo.GetRefreshableCredential(It.IsAny<string>(), It.IsAny<string>()))
      .ReturnsAsync(Result.Ok(refreshModel));

    _nukiCredentialRepositoryMock
      .Setup(repo =>
        repo.ConfirmDraft(It.IsAny<NukiCredentialDraftModel>(), It.IsAny<NukiRefreshableCredentialModel>()))
      .ReturnsAsync(Result.Ok(finalModel));

    _nukiCredentialRepositoryMock
      .Setup(repo => repo.DeleteDraftByUserId(It.IsAny<Guid>()))
      .ReturnsAsync(Result.Ok);

    var result = await _interactor.Handle("acode", userId);

    _kerberoConfigurationRepositoryMock.Verify();
    _nukiCredentialRepositoryMock.Verify();
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().BeEquivalentTo(finalModel);
  }
}
