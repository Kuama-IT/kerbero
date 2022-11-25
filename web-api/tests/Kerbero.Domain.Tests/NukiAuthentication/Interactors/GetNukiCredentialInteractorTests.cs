using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiAuthentication.Dtos;
using Kerbero.Domain.NukiAuthentication.Interactors;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Moq;

namespace Kerbero.Domain.Tests.NukiAuthentication.Interactors;

public class GetNukiCredentialInteractorTests
{
  private readonly GetNukiCredentialInteractor _interactor;

  private readonly Mock<INukiCredentialRepository>
    _nukiCredentialRepositoryMock = new Mock<INukiCredentialRepository>();

  private readonly Mock<INukiOAuthRepository> _nukiOAuthRepositoryMock = new Mock<INukiOAuthRepository>();

  public GetNukiCredentialInteractorTests()
  {
    _interactor =
      new GetNukiCredentialInteractor(_nukiCredentialRepositoryMock.Object, _nukiOAuthRepositoryMock.Object);
  }

  [Fact]
  public async Task Handle_RefreshToken_Success_Test()
  {
    var nukiCredential = new NukiCredential()
    {
      Id = 1,
      Token = "VALID_TOKEN",
      RefreshToken = "VALID_REFRESH_TOKEN",
      TokenExpiringTimeInSeconds = DateTime.Now.AddSeconds(-2592000).Second,
      ClientId = "VALID_CLIENT_ID",
      TokenType = "bearer",
    };

    _nukiCredentialRepositoryMock.Setup(c => c.GetById(It.IsAny<int>()))
      .ReturnsAsync(nukiCredential);
    _nukiOAuthRepositoryMock.Setup(c => c.RefreshNukiOAuth(It.IsAny<NukiOAuthRequest>()))
      .ReturnsAsync(() =>
      {
        nukiCredential.TokenExpiringTimeInSeconds = DateTime.Now.AddSeconds(2592000).Second;
        return nukiCredential;
      });
    
    _nukiCredentialRepositoryMock.Setup(c => c.Update(It.IsAny<NukiCredential>()))
      .Returns(Task.FromResult(Result.Ok(nukiCredential)));

    var response = await _interactor.Handle(
      new GetNukiCredentialParams { NukiCredentialId = 0 }
      );

    response.IsSuccess.Should().BeTrue();
    _nukiCredentialRepositoryMock.Verify(c => c.GetById(It.IsAny<int>()));
    _nukiOAuthRepositoryMock.Verify(c =>
      c.RefreshNukiOAuth(It.Is<NukiOAuthRequest>(n => n.RefreshToken == "VALID_REFRESH_TOKEN")));
    _nukiCredentialRepositoryMock.Verify();
  }

  [Fact]
  public async Task Handle_NoAccountFound_Test()
  {
    _nukiCredentialRepositoryMock.Setup(c => c.GetById(It.IsAny<int>()))
      .ReturnsAsync(Result.Fail(new UnauthorizedAccessError()));

    var result = await _interactor.Handle(new GetNukiCredentialParams
    {
      NukiCredentialId = 12
    });

    result.IsFailed.Should().BeTrue();
    _nukiCredentialRepositoryMock.Verify(c => c.GetById(It.Is<int>(p => p.Equals(12))));
    result.Errors.First().Should().BeEquivalentTo(new UnauthorizedAccessError());
  }

  [Fact]
  public async Task Handle_RefreshToken_Fail_Test()
  {
    var nukiCredential = new NukiCredential()
    {
      Id = 1,
      Token = "VALID_TOKEN",
      RefreshToken = "VALID_REFRESH_TOKEN",
      TokenExpiringTimeInSeconds = DateTime.Now.AddSeconds(-2592000).Second,
      ClientId = "VALID_CLIENT_ID",
      TokenType = "bearer",
    };
    
    _nukiCredentialRepositoryMock.Setup(c => c.GetById(It.IsAny<int>()))
      .ReturnsAsync(nukiCredential);
    _nukiOAuthRepositoryMock.Setup(c => c.RefreshNukiOAuth(It.IsAny<NukiOAuthRequest>()))
      .Returns(async () => await Task.FromResult(Result.Fail(new ExternalServiceUnreachableError())));

    var response = await _interactor.Handle(new GetNukiCredentialParams
    {
      NukiCredentialId = 0
    });

    response.IsFailed.Should().BeTrue();
    _nukiCredentialRepositoryMock.Verify(c => c.GetById(It.IsAny<int>()));
    _nukiOAuthRepositoryMock.Verify(c =>
      c.RefreshNukiOAuth(It.Is<NukiOAuthRequest>(n => n.RefreshToken == "VALID_REFRESH_TOKEN")));
    response.Errors.First().Should().BeEquivalentTo(new ExternalServiceUnreachableError());
  }
}