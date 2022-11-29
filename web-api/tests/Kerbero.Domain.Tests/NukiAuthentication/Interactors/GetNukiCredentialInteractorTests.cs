using FluentAssertions;
using FluentResults;
using Kerbero.Domain.NukiAuthentication.Dtos;
using Kerbero.Domain.NukiAuthentication.Errors;
using Kerbero.Domain.NukiAuthentication.Interactors;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Moq;

namespace Kerbero.Domain.Tests.NukiAuthentication.Interactors;

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
