using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiAuthentication.Errors;
using Kerbero.Domain.NukiAuthentication.Interactors;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Models.ExternalResponses;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Moq;

namespace Kerbero.Domain.Tests.NukiAuthentication.Interactors;

public class CreateNukiAccountInteractorTests
{
  private readonly CreateNukiAccountInteractor _interactor;

  private readonly Mock<INukiAccountExternalRepository> _nukiExternalRepository =
    new Mock<INukiAccountExternalRepository>();

  private readonly Mock<INukiCredentialRepository> _nukiPersistentRepository = new Mock<INukiCredentialRepository>();

  private readonly Mock<INukiCredentialDraftRepository> _nukiAccountDraftRepository =
    new Mock<INukiCredentialDraftRepository>();


  public CreateNukiAccountInteractorTests()
  {
    _interactor = new CreateNukiAccountInteractor(
      nukiCredentialRepository: _nukiPersistentRepository.Object,
      nukiAccountExternalRepository: _nukiExternalRepository.Object,
      nukiCredentialDraftRepository: _nukiAccountDraftRepository.Object
    );
  }

  // Handle should create an entity from an input DTO and upload it into the DB.
  [Fact]
  public async Task Handle_ReturnASuccessfulResponse_Test()
  {
    // Arrange
    var nukiAccountDto = new NukiAccountExternalResponse
    {
      Token = "VALID_TOKEN",
      RefreshToken = "VALID_REFRESH_TOKEN",
      ClientId = "VALID_CLIENT_ID",
      TokenType = "bearer",
      TokenExpiresIn = 2592000,
    };

    var nukiAccountEntity = new NukiAccount
    {
      Id = 1,
      Token = "VALID_TOKEN",
      RefreshToken = "VALID_REFRESH_TOKEN",
      TokenExpiringTimeInSeconds = 2592000,
      ClientId = "VALID_CLIENT_ID",
      TokenType = "bearer",
    };

    var nukiAccountDraft = new NukiCredentialDraft(
      ClientId: "clientId",
      RedirectUrl: "uri.ToString()",
      UserId: new Guid()
    );

    _nukiAccountDraftRepository.Setup(it => it.GetByClientId(It.IsAny<string>()))
      .ReturnsAsync(nukiAccountDraft);
    _nukiAccountDraftRepository.Setup(it => it.DeleteByClientId(It.IsAny<string>()))
      .ReturnsAsync(Result.Ok());

    _nukiExternalRepository.Setup(c => c.GetNukiAccount(
        It.IsAny<NukiAccountExternalRequest>()))
      .Returns(() => Task.FromResult(Result.Ok(nukiAccountDto)));

    _nukiPersistentRepository.Setup(c =>
      c.Create(It.IsAny<NukiAccount>())).ReturnsAsync(Result.Ok(nukiAccountEntity));

    // Act
    var nukiAccountPresentationDto = await _interactor.Handle(new CreateNukiAccountPresentationRequest
      { ClientId = "VALID_CLIENT_ID", Code = "VALID_CODE" });

    // Assert
    _nukiExternalRepository.Verify(c => c.GetNukiAccount(
      It.Is<NukiAccountExternalRequest>(s =>
        s.ClientId.Contains("VALID_CLIENT_ID") &&
        s.Code!.Contains("VALID_CODE"))));
    _nukiPersistentRepository.Verify(c => c
      .Create(It.Is<NukiAccount>(account =>
        account.Token == nukiAccountEntity.Token &&
        account.RefreshToken == nukiAccountEntity.RefreshToken &&
        account.TokenExpiringTimeInSeconds == nukiAccountEntity.TokenExpiringTimeInSeconds &&
        account.TokenType == nukiAccountEntity.TokenType &&
        account.ClientId == nukiAccountEntity.ClientId)));
    nukiAccountPresentationDto.Should().BeOfType<Result<CreateNukiAccountPresentationResponse>>();
    nukiAccountPresentationDto.Value.Should().BeEquivalentTo(new CreateNukiAccountPresentationResponse
    {
      Id = 1,
      ClientId = "VALID_CLIENT_ID"
    });
  }

  [Theory]
  [MemberData(nameof(ExternalErrorToTest))]
  public async Task Handle_ExternalReturnAnError_Test(KerberoError error)
  {
    // Arrange
    var nukiAccountDraft = new NukiCredentialDraft(
      ClientId: "clientId",
      RedirectUrl: "uri.ToString()",
      UserId: new Guid()
    );
    
    _nukiAccountDraftRepository.Setup(it => it.GetByClientId(It.IsAny<string>()))
      .ReturnsAsync(nukiAccountDraft);
    _nukiAccountDraftRepository.Setup(it => it.DeleteByClientId(It.IsAny<string>()))
      .ReturnsAsync(Result.Ok());
    
    _nukiExternalRepository.Setup(c => c.GetNukiAccount(
        It.IsAny<NukiAccountExternalRequest>()))
      .Returns(async () => await Task.FromResult(Result.Fail(error)));

    // Act 
    var ex = await _interactor.Handle(new CreateNukiAccountPresentationRequest
      { ClientId = "VALID_CLIENT_ID", Code = "VALID_CODE" });
    // Assert
    ex.IsFailed.Should().BeTrue();
    ex.Errors.FirstOrDefault()!.Should().BeEquivalentTo(error);
  }

  public static IEnumerable<object[]> ExternalErrorToTest =>
    new List<object[]>
    {
      new object[] { new ExternalServiceUnreachableError() },
      new object[] { new UnableToParseResponseError() },
      new object[] { new UnauthorizedAccessError() },
      new object[] { new KerberoError() },
      new object[] { new InvalidParametersError("VALID_CLIENT_ID") }
    };

  [Theory]
  [MemberData(nameof(PersistentErrorToTest))]
  public async Task Handle_PersistentReturnAnError_Test(KerberoError error)
  {
    // Arrange
    var nukiAccountDraft = new NukiCredentialDraft(
      ClientId: "clientId",
      RedirectUrl: "uri.ToString()",
      UserId: new Guid()
    );
    
    _nukiAccountDraftRepository.Setup(it => it.GetByClientId(It.IsAny<string>()))
      .ReturnsAsync(nukiAccountDraft);
    _nukiAccountDraftRepository.Setup(it => it.DeleteByClientId(It.IsAny<string>()))
      .ReturnsAsync(Result.Ok());

    var nukiAccountDto = new NukiAccountExternalResponse
    {
      Token = "VALID_TOKEN",
      RefreshToken = "VALID_REFRESH_TOKEN",
      ClientId = "VALID_CLIENT_ID",
      TokenType = "bearer",
      TokenExpiresIn = 2592000,
    };
    _nukiExternalRepository.Setup(c => c.GetNukiAccount(
        It.IsAny<NukiAccountExternalRequest>()))
      .Returns(async () => await Task.FromResult(Result.Ok(nukiAccountDto)));
    _nukiPersistentRepository.Setup(c =>
      c.Create(It.IsAny<NukiAccount>())).ReturnsAsync(Result.Fail(error));

    // Act 
    var ex = await _interactor.Handle(new CreateNukiAccountPresentationRequest
      { ClientId = "VALID_CLIENT_ID", Code = "VALID_CODE" });
    // Assert
    ex.IsFailed.Should().BeTrue();
    ex.Errors.FirstOrDefault()!.Should().BeEquivalentTo(error);
  }

  public static IEnumerable<object[]> PersistentErrorToTest =>
    new List<object[]>
    {
      new object[] { new NukiAccountNotFoundError() },
      new object[] { new DuplicateEntryError("Nuki account") },
      new object[] { new PersistentResourceNotAvailableError() }
    };
}
