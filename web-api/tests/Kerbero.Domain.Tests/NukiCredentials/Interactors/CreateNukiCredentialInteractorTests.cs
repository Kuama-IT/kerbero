using FluentAssertions;
using FluentResults;
using Kerbero.Domain.NukiCredentials.Interactors;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.NukiCredentials.Repositories;
using Kerbero.Domain.SmartLocks.Repositories;
using Moq;

namespace Kerbero.Domain.Tests.NukiCredentials.Interactors;

public class CreateNukiCredentialInteractorTests
{
  private readonly CreateNukiCredentialInteractor _createNukiCredentialInteractor;

  private readonly Mock<INukiCredentialRepository> _nukiCredentialRepositoryMock = new();
  private readonly Mock<INukiSmartLockRepository> _nukiSmartLockRepositoryMock = new();

  public CreateNukiCredentialInteractorTests()
  {
    _createNukiCredentialInteractor = new CreateNukiCredentialInteractor(
      nukiCredentialRepository: _nukiCredentialRepositoryMock.Object
    );
  }

  [Fact]
  public async Task CreateNukiCredentials_WithValidParameters()
  {
    // Arrange
    var nukiCredentials = new NukiCredentialModel()
    {
      Id = 1,
      Token = "VALID_TOKEN",
      NukiEmail = "test@nuki.com"
    };

    _nukiCredentialRepositoryMock.Setup(repo => repo.ValidateNotRefreshableApiToken(It.IsAny<string>()))
      .ReturnsAsync(Result.Ok);
    _nukiCredentialRepositoryMock.Setup(c => c.Create(It.IsAny<NukiCredentialModel>(), It.IsAny<Guid>()))
      .ReturnsAsync(() => Result.Ok(nukiCredentials));

    // Act
    var nukiCredentialDto = await _createNukiCredentialInteractor.Handle(
      token: "VALID_TOKEN", userId: new Guid()
    );

    // Assert
    _nukiCredentialRepositoryMock.Verify();
    nukiCredentialDto.Should().BeOfType<Result<NukiCredentialModel>>();
    nukiCredentialDto.Should().BeEquivalentTo(Result.Ok(new NukiCredentialModel()
    {
      Id = 1,
      Token = "VALID_TOKEN",
      NukiEmail = "test@nuki.com"
    }));
  }
}