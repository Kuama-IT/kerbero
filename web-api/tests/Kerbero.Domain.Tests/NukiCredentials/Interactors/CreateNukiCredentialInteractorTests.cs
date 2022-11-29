using FluentAssertions;
using FluentResults;
using Kerbero.Domain.NukiCredentials.Dtos;
using Kerbero.Domain.NukiCredentials.Interactors;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.NukiCredentials.Repositories;
using Moq;

namespace Kerbero.Domain.Tests.NukiAuthentication.Interactors;

public class CreateNukiCredentialInteractorTests
{
  private readonly CreateNukiCredentialInteractor _createNukiCredentialInteractor;

  private readonly Mock<INukiCredentialRepository> _nukiCredentialRepositoryMock = new();

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
    };

    _nukiCredentialRepositoryMock.Setup(c => c.Create(It.IsAny<NukiCredentialModel>(), It.IsAny<Guid>()))
      .ReturnsAsync(() => Result.Ok(nukiCredentials));

    // Act
    var nukiCredentialDto = await _createNukiCredentialInteractor.Handle(
      new CreateNukiCredentialParams() { Token = "VALID_TOKEN", UserId = new Guid() }
    );

    // Assert
    _nukiCredentialRepositoryMock.Verify();
    nukiCredentialDto.Should().BeOfType<Result<NukiCredentialDto>>();
    nukiCredentialDto.Should().BeEquivalentTo(Result.Ok(new NukiCredentialDto()
    {
      Id = 1,
      Token = "VALID_TOKEN"
    }));
  }
}
