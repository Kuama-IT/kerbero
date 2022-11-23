using FluentAssertions;
using FluentResults;
using Kerbero.Domain.NukiCredentials.Dtos;
using Kerbero.Domain.NukiCredentials.Errors;
using Kerbero.Domain.NukiCredentials.Interactors;
using Kerbero.Domain.NukiCredentials.Repositories;
using Moq;

namespace Kerbero.Domain.Tests.NukiCredentials.Interactors;

public class GetNukiCredentialInteractorTests
{
  private readonly GetNukiCredentialInteractor _interactor;

  private readonly Mock<INukiCredentialRepository>
    _nukiCredentialRepositoryMock = new();

  public GetNukiCredentialInteractorTests()
  {
    _interactor =
      new GetNukiCredentialInteractor(_nukiCredentialRepositoryMock.Object);
  }
  
  [Fact]
  public async Task Handle_NoAccountFound_Test()
  {
    _nukiCredentialRepositoryMock.Setup(c => c.GetById(It.IsAny<int>()))
      .ReturnsAsync(Result.Fail(new NukiCredentialNotFoundError()));

    var result = await _interactor.Handle(new GetNukiCredentialParams
    {
      NukiCredentialId = 12
    });

    result.IsFailed.Should().BeTrue();
    _nukiCredentialRepositoryMock.Verify(c => c.GetById(It.Is<int>(p => p.Equals(12))));
    result.Errors.First().Should().BeEquivalentTo(new NukiCredentialNotFoundError());
  }
}
