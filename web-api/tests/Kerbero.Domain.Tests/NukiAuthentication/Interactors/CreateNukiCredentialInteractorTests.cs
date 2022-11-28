using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiAuthentication.Dtos;
using Kerbero.Domain.NukiAuthentication.Errors;
using Kerbero.Domain.NukiAuthentication.Interactors;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Moq;

namespace Kerbero.Domain.Tests.NukiAuthentication.Interactors;

public class CreateNukiCredentialInteractorTests
{
  private readonly CreateNukiCredentialInteractor _createNukiCredentialInteractor;

  private readonly Mock<INukiOAuthRepository> _nukiOAuthRepositoryMock = new();
  private readonly Mock<INukiCredentialRepository> _nukiCredentialRepositoryMock = new();
  private readonly Mock<INukiCredentialDraftRepository> _nukiCredentialDraftRepositoryMock = new();


  public CreateNukiCredentialInteractorTests()
  {
    _createNukiCredentialInteractor = new CreateNukiCredentialInteractor(
      nukiCredentialRepository: _nukiCredentialRepositoryMock.Object,
      nukiOAuthRepository: _nukiOAuthRepositoryMock.Object,
      nukiCredentialDraftRepository: _nukiCredentialDraftRepositoryMock.Object
    );
  }

  [Fact]
  public async Task It_Turns_A_Draft_Credential_Into_A_Nuki_Credential()
  {
    // Arrange
    var nukiCredentials = new NukiCredential()
    {
      Id = 1,
      Token = "VALID_TOKEN",
      RefreshToken = "VALID_REFRESH_TOKEN",
      ClientId = "VALID_CLIENT_ID",
      TokenType = "bearer",
    };

    var nukiCredentialDraft = new NukiCredentialDraft(
      ClientId: "clientId",
      RedirectUrl: "uri.ToString()",
      UserId: new Guid()
    );

    _nukiCredentialDraftRepositoryMock.Setup(it => it.GetByClientId(It.IsAny<string>()))
      .ReturnsAsync(nukiCredentialDraft);

    _nukiCredentialDraftRepositoryMock.Setup(it => it.DeleteByClientId(It.IsAny<string>()))
      .ReturnsAsync(Result.Ok());

    _nukiOAuthRepositoryMock.Setup(c => c.Authenticate(It.IsAny<string>(), It.IsAny<string>()))
      .ReturnsAsync(() => Result.Ok(nukiCredentials));

    _nukiCredentialRepositoryMock.Setup(c => c.Create(It.IsAny<NukiCredential>(), It.IsAny<Guid>()))
      .ReturnsAsync(() => Result.Ok(nukiCredentials));

    // Act
    var nukiCredentialDto = await _createNukiCredentialInteractor.Handle(
      new CreateNukiCredentialParams() { ClientId = "VALID_CLIENT_ID", Code = "VALID_CODE" }
    );

    // Assert
    _nukiOAuthRepositoryMock.Verify();
    _nukiCredentialRepositoryMock.Verify();
    nukiCredentialDto.Should().BeOfType<Result<NukiCredentialDto>>();
    nukiCredentialDto.Should().BeEquivalentTo(Result.Ok(new NukiCredentialDto()
    {
      Id = 1,
      ClientId = "VALID_CLIENT_ID",
      Token = "VALID_TOKEN"
    }));
  }

  [Fact]
  public async Task It_Handle_Nuki_Auth_Error()
  {
    // Arrange
    var tNukiCredentialsDraft = new NukiCredentialDraft(
      ClientId: "clientId",
      RedirectUrl: "uri.ToString()",
      UserId: new Guid()
    );

    var tError = new ExternalServiceUnreachableError();

    _nukiCredentialDraftRepositoryMock.Setup(it => it.GetByClientId(It.IsAny<string>()))
      .ReturnsAsync(tNukiCredentialsDraft);
    _nukiCredentialDraftRepositoryMock.Setup(it => it.DeleteByClientId(It.IsAny<string>()))
      .ReturnsAsync(Result.Ok());


    _nukiOAuthRepositoryMock.Setup(c => c.Authenticate(It.IsAny<string>(), It.IsAny<string>()))
      .ReturnsAsync(() => Result.Fail(tError));

    // Act 
    var actual = await _createNukiCredentialInteractor.Handle(
      new CreateNukiCredentialParams() { ClientId = "VALID_CLIENT_ID", Code = "VALID_CODE" }
    );

    // Assert
    var expected = Result.Fail(new ExternalServiceUnreachableError());

    actual.IsFailed.Should().BeTrue();
    actual.Should().BeEquivalentTo(expected);
  }
}