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
  private readonly CreateNukiCredentialInteractor _interactor;

  private readonly Mock<INukiOAuthRepository> _nukiOAuthRepositoryMock =
    new Mock<INukiOAuthRepository>();

  private readonly Mock<INukiCredentialRepository>
    _nukiCredentialRepositoryMock = new Mock<INukiCredentialRepository>();

  private readonly Mock<INukiCredentialDraftRepository> _nukiCredentialDraftRepositoryMock =
    new Mock<INukiCredentialDraftRepository>();


  public CreateNukiCredentialInteractorTests()
  {
    _interactor = new CreateNukiCredentialInteractor(
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

    _nukiOAuthRepositoryMock.Setup(c => c.Authenticate(
        It.IsAny<string>(), It.IsAny<string>()))
      .Returns(() => Task.FromResult(Result.Ok(nukiCredentials)));

    _nukiCredentialRepositoryMock.Setup(c =>
      c.Create(It.IsAny<NukiCredential>())).ReturnsAsync(() =>
    {
      nukiCredentials.Id = 1;
      return Result.Ok(nukiCredentials);
    });

    // Act
    var nukiCredentialDto = await _interactor.Handle(
      new CreateNukiCredentialParams() { ClientId = "VALID_CLIENT_ID", Code = "VALID_CODE" }
    );

    // Assert
    _nukiOAuthRepositoryMock.Verify();
    _nukiCredentialRepositoryMock.Verify();
    nukiCredentialDto.Should().BeOfType<Result<NukiCredentialDto>>();
    nukiCredentialDto.Value.Should().BeEquivalentTo(new NukiCredentialDto()
    {
      Id = 1,
      ClientId = "VALID_CLIENT_ID",
      Token = "VALID_TOKEN"
    });
  }

  [Fact]
  public async Task It_Handle_Nuki_Auth_Error()
  {
    // Arrange
    var nukiAccountDraft = new NukiCredentialDraft(
      ClientId: "clientId",
      RedirectUrl: "uri.ToString()",
      UserId: new Guid()
    );

    _nukiCredentialDraftRepositoryMock.Setup(it => it.GetByClientId(It.IsAny<string>()))
      .ReturnsAsync(nukiAccountDraft);
    _nukiCredentialDraftRepositoryMock.Setup(it => it.DeleteByClientId(It.IsAny<string>()))
      .ReturnsAsync(Result.Ok());

    var error = new ExternalServiceUnreachableError();

    _nukiOAuthRepositoryMock.Setup(
        c => c.Authenticate(It.IsAny<string>(), It.IsAny<string>())
      )
      .Returns(async () => await Task.FromResult(Result.Fail(error)));

    // Act 
    var ex = await _interactor.Handle(
      new CreateNukiCredentialParams() { ClientId = "VALID_CLIENT_ID", Code = "VALID_CODE" }
    );
    // Assert
    ex.IsFailed.Should().BeTrue();
    ex.Errors.FirstOrDefault()!.Should().BeEquivalentTo(error);
  }
}