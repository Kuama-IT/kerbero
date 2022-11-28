using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiAuthentication.Dtos;
using Kerbero.Domain.NukiAuthentication.Interactors;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Moq;

namespace Kerbero.Domain.Tests.NukiAuthentication.Interactors;

public class CreateNukiCredentialDraftInteractorTests
{
  private readonly CreateNukiCredentialDraftInteractor _createNukiCredentialDraftInteractor;

  private readonly Mock<INukiOAuthRepository> _nukiOAuthRepositoryMock = new();

  private readonly Mock<INukiCredentialDraftRepository> _nukiAccountDraftRepositoryMock = new();

  public CreateNukiCredentialDraftInteractorTests()
  {
    _createNukiCredentialDraftInteractor =
      new CreateNukiCredentialDraftInteractor(
        _nukiOAuthRepositoryMock.Object,
        _nukiAccountDraftRepositoryMock.Object
      );
  }

  [Fact]
  public async Task It_Creates_A_Nuki_Credential_Draft()
  {
    // Arrange

    var interactorParams = new CreateNukiCredentialDraftParams(clientId: "clientId", userId: new Guid());

    var uri = new Uri("http://test.test");
    _nukiOAuthRepositoryMock.Setup(c => c.GetOAuthRedirectUri(It.IsAny<string>()))
      .Returns(uri);

    _nukiAccountDraftRepositoryMock.Setup(it => it.Create(It.IsAny<NukiCredentialDraft>()))
      .ReturnsAsync(Result.Ok());

    // Act
    var nukiAccountDraftDto = await
      _createNukiCredentialDraftInteractor.Handle(interactorParams);

    // Assert
    _nukiAccountDraftRepositoryMock.Verify();

    _createNukiCredentialDraftInteractor.Should()
      .BeAssignableTo<InteractorAsync<CreateNukiCredentialDraftParams,
        NukiCredentialDraftDto>>();

    _nukiOAuthRepositoryMock.Verify(
      c => c.GetOAuthRedirectUri(
        It.Is<string>(s => s == interactorParams.ClientId)
      )
    );

    nukiAccountDraftDto.Should().BeOfType<Result<NukiCredentialDraftDto>>();
    nukiAccountDraftDto.Value.RedirectUrl.Should().BeEquivalentTo(uri.ToString());
    nukiAccountDraftDto.Value.UserId.Should().Be(interactorParams.UserId);
    nukiAccountDraftDto.Value.ClientId.Should().Be(interactorParams.ClientId);
  }

  [Fact]
  public async Task Handle_Return_ErrorResult()
  {
    // Arrange
    var error = new InvalidParametersError("options");
    _nukiOAuthRepositoryMock.Setup(c => c.GetOAuthRedirectUri(It.IsAny<string>()))
      .Returns(Result.Fail(error));

    var result = await _createNukiCredentialDraftInteractor.Handle(
      new CreateNukiCredentialDraftParams("VALID_CLIENT_ID", userId: new Guid()));

    result.IsFailed.Should().BeTrue();
    result.Errors.Single(e => e.Equals(error)).Should().NotBeNull().And.BeEquivalentTo(error);
  }
}