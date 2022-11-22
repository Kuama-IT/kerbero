using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiAuthentication.Dtos;
using Kerbero.Domain.NukiAuthentication.Interactors;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Models.ExternalResponses;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Moq;

namespace Kerbero.Domain.Tests.NukiAuthentication.Interactors;

public class CreateNukiAccountDraftInteractorTests
{
  private readonly CreateNukiAccountDraftInteractor _createNukiAccountDraftInteractor;

  private readonly Mock<INukiOAuthRepository> _nukiExternalRepository =
    new Mock<INukiOAuthRepository>();

  private readonly Mock<INukiCredentialRepository> _nukiPersistentRepository = new Mock<INukiCredentialRepository>();

  private readonly Mock<INukiCredentialDraftRepository> _nukiAccountDraftRepository =
    new Mock<INukiCredentialDraftRepository>();

  public CreateNukiAccountDraftInteractorTests()
  {
    _createNukiAccountDraftInteractor =
      new CreateNukiAccountDraftInteractor(
        _nukiExternalRepository.Object,
        _nukiPersistentRepository.Object,
        _nukiAccountDraftRepository.Object
      );
  }

  [Fact]
  public async Task Handle_Success()
  {
    // Arrange
    var clientId = "VALID_CLIENT_ID";
    var nukiAccountEntity = new NukiAccount
    {
      ClientId = clientId
    };
    var uri = new Uri("http://api.nuki.io/oauth/authorize?response_type=code" +
                      "&client_id=v7kn_NX7vQ7VjQdXFGK43g" +
                      "&redirect_uri=https%3A%2F%2Ftest.com%2Fnuki%2Fcode%2Fv7kn_NX7vQ7VjQdXFGK43g" +
                      "&scope=account notification smartlock smartlock.readOnly smartlock.action smartlock.auth smartlock.config smartlock.log");
    _nukiExternalRepository.Setup(c => c.GetOAuthRedirectUri(It.IsAny<NukiAccountBuildUriForCodeExternalRequest>()))
      .Returns(new NukiAccountBuildUriForCodeExternalResponse(uri));
    
    _nukiPersistentRepository.Setup(c => c.Create(It.IsAny<NukiAccount>()))
      .ReturnsAsync(() =>
      {
        nukiAccountEntity.Id = 1;
        return Result.Ok(nukiAccountEntity);
      });

    _nukiAccountDraftRepository.Setup(it => it.Create(It.IsAny<NukiCredentialDraft>()))
      .ReturnsAsync(Result.Ok(new NukiCredentialDraft(
        ClientId: clientId,
        RedirectUrl: uri.ToString(),
        UserId: new Guid()
      )));

    // Act
    var nukiAccountDraft = await
      _createNukiAccountDraftInteractor.Handle(
        new CreateNukiAccountDraftParams(clientId: clientId, userId: new Guid()));

    // Assert
    _nukiAccountDraftRepository.Verify();

    _createNukiAccountDraftInteractor.Should()
      .BeAssignableTo<InteractorAsync<CreateNukiAccountDraftParams,
        NukiAccountDraftDto>>();

    _nukiExternalRepository.Verify(c => c.GetOAuthRedirectUri(It.Is<NukiAccountBuildUriForCodeExternalRequest>(s =>
      s.ClientId.Equals("VALID_CLIENT_ID"))));


    nukiAccountDraft.Should().BeOfType<Result<NukiAccountDraftDto>>();
    nukiAccountDraft.Value.RedirectUrl.Should().BeEquivalentTo(uri.ToString());
  }

  [Theory]
  [MemberData(nameof(PersistentErrorToTest))]
  public async Task Handle_Return_ErrorResult(KerberoError error)
  {
    // Arrange
    var uri = new Uri("http://api.nuki.io/oauth/authorize?response_type=code" +
                      "&client_id=v7kn_NX7vQ7VjQdXFGK43g" +
                      "&redirect_uri=https%3A%2F%2Ftest.com%2Fnuki%2Fcode%2Fv7kn_NX7vQ7VjQdXFGK43g" +
                      "&scope=account notification smartlock smartlock.readOnly smartlock.action smartlock.auth smartlock.config smartlock.log");
    _nukiExternalRepository.Setup(c => c.GetOAuthRedirectUri(It.IsAny<NukiAccountBuildUriForCodeExternalRequest>()))
      .Returns(Result.Ok(new NukiAccountBuildUriForCodeExternalResponse(uri)));

    _nukiAccountDraftRepository.Setup(c => c.Create(It.IsAny<NukiCredentialDraft>()))
      .ReturnsAsync(Result.Fail(error));

    var result = await _createNukiAccountDraftInteractor.Handle(
      new CreateNukiAccountDraftParams("VALID_CLIENT_ID", userId: new Guid()));

    result.IsFailed.Should().BeTrue();
    result.Errors.Single(e => e.Equals(error)).Should().NotBeNull().And.BeEquivalentTo(error);
  }

  public static IEnumerable<object[]> PersistentErrorToTest =>
    new List<object[]>
    {
      new object[] { new DuplicateEntryError("Nuki account") },
      new object[] { new PersistentResourceNotAvailableError() }
    };
}