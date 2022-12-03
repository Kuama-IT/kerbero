using FluentAssertions;
using FluentResults;
using Kerbero.Domain.NukiCredentials.Interactors;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.NukiCredentials.Repositories;
using Moq;

namespace Kerbero.Domain.Tests.NukiCredentials.Interactors;

public class CreateNukiCredentialDraftInteractorTests
{
  private readonly CreateNukiCredentialDraftInteractor _interactor;
  private readonly Mock<INukiCredentialRepository> _nukiCredentialRepositoryMock = new();

  public CreateNukiCredentialDraftInteractorTests()
  {
    _interactor = new CreateNukiCredentialDraftInteractor(_nukiCredentialRepositoryMock.Object);
  }

  [Fact]
  async Task It_Stores_A_Draft_Credential()
  {
    var userId = Guid.NewGuid();
    var model = new NukiCredentialDraftModel(UserId: userId);
    _nukiCredentialRepositoryMock
      .Setup(repo => repo.CreateDraft(It.IsAny<NukiCredentialDraftModel>()))
      .ReturnsAsync(Result.Ok());

    var result = await _interactor.Handle(userId);

    _nukiCredentialRepositoryMock.Verify();
    result.IsSuccess.Should().BeTrue();
  }
}