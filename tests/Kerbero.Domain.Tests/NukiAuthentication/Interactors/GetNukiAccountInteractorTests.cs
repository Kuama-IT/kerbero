using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiActions.Repositories;
using Kerbero.Domain.NukiAuthentication.Entities;
using Kerbero.Domain.NukiAuthentication.Interactors;
using Kerbero.Domain.NukiAuthentication.Models.ExternalRequests;
using Kerbero.Domain.NukiAuthentication.Models.ExternalResponses;
using Kerbero.Domain.NukiAuthentication.Models.PresentationRequests;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Moq;

namespace Kerbero.Domain.Tests.NukiAuthentication.Interactors;

public class GetNukiAccountInteractorTests
{
    private readonly AuthenticateNukiAccountInteractor _interactor;
    private readonly Mock<INukiAccountPersistentRepository> _persistent;
    private readonly Mock<INukiAccountExternalRepository> _nukiAccountClient;

    public GetNukiAccountInteractorTests()
    {
        _nukiAccountClient = new Mock<INukiAccountExternalRepository>();
        _persistent = new Mock<INukiAccountPersistentRepository>();
        _interactor = new AuthenticateNukiAccountInteractor(_persistent.Object, _nukiAccountClient.Object);
    }

    [Fact]
    public async Task Handle_Success_Test()
    {
        _persistent.Setup(c => c.GetAccount(It.IsAny<int>()))
            .Returns(new NukiAccount
            {
                Id = 1,
                Token = "VALID_TOKEN",
                RefreshToken = "VALID_REFRESH_TOKEN",
                TokenExpiringTimeInSeconds = 2592000,
                ExpiryDate = DateTime.Now.AddSeconds(2592000),
                ClientId = "VALID_CLIENT_ID",
                TokenType = "bearer",
            });
        
        var response = await _interactor.Handle(new AuthenticateRepositoryPresentationRequest
        {
            NukiAccountId = 0
        });

        response.IsSuccess.Should().BeTrue();
        _persistent.Verify(c => c.GetAccount(It.IsAny<int>()));
    }    
    
    [Fact]
    public async Task Handle_RefreshToken_Success_Test()
    {
        var nukiAccount = new NukiAccount
        {
            Id = 1,
            Token = "VALID_TOKEN",
            RefreshToken = "VALID_REFRESH_TOKEN",
            TokenExpiringTimeInSeconds = 2592000,
            ExpiryDate = DateTime.Now.AddSeconds(-2592000),
            ClientId = "VALID_CLIENT_ID",
            TokenType = "bearer",
        };

        _persistent.Setup(c => c.GetAccount(It.IsAny<int>()))
            .Returns(nukiAccount);
        _nukiAccountClient.Setup(c => c.RefreshToken(It.IsAny<NukiAccountExternalRequest>()))
            .Returns(Task.FromResult(Result.Ok(new NukiAccountExternalResponse
            {
                Token = "VALID_TOKEN",
                RefreshToken = "VALID_REFRESH_TOKEN",
                TokenExpiresIn = 2592000,
                ClientId = "VALID_CLIENT_ID",
                TokenType = "bearer",
            })));
        _persistent.Setup(c => c.Update(It.IsAny<NukiAccount>()))
            .Returns(Task.FromResult(Result.Ok(nukiAccount)));
        
        var response = await _interactor.Handle(new AuthenticateRepositoryPresentationRequest
        {
            NukiAccountId = 0
        });

        response.IsSuccess.Should().BeTrue();
        _persistent.Verify(c => c.GetAccount(It.IsAny<int>()));
        _nukiAccountClient.Verify(c => 
            c.RefreshToken(It.Is<NukiAccountExternalRequest>(n => n.RefreshToken == "VALID_REFRESH_TOKEN")));
        _persistent.Verify(c => c.Update(It.Is<NukiAccount>(p => 
                p.Token == "VALID_TOKEN" &&
                p.RefreshToken == "VALID_REFRESH_TOKEN" &&
                p.TokenExpiringTimeInSeconds == 2592000 &&
                p.ClientId == "VALID_CLIENT_ID" &&
                p.TokenType == "bearer"
        )));
    }

    [Fact]
    public async Task Handle_NoAccountFound_Test()
    {
        _persistent.Setup(c => c.GetAccount(It.IsAny<int>()))
            .Returns( () =>  Result.Fail(new UnauthorizedAccessError()));
        
        var result = await _interactor.Handle(new AuthenticateRepositoryPresentationRequest
        {
            NukiAccountId = 12
        });

        result.IsFailed.Should().BeTrue();
        _persistent.Verify(c => c.GetAccount(It.Is<int>(p => p.Equals(12))));
        result.Errors.First().Should().BeEquivalentTo(new UnauthorizedAccessError());
    }
    
    [Fact]
    public async Task Handle_RefreshToken_Fail_Test()
    {
        var nukiAccount = new NukiAccount
        {
            Id = 1,
            Token = "VALID_TOKEN",
            RefreshToken = "VALID_REFRESH_TOKEN",
            TokenExpiringTimeInSeconds = 2592000,
            ExpiryDate = DateTime.Now.AddSeconds(-2592000),
            ClientId = "VALID_CLIENT_ID",
            TokenType = "bearer",
        };
        _persistent.Setup(c => c.GetAccount(It.IsAny<int>()))
            .Returns(nukiAccount);
        _nukiAccountClient.Setup(c => c.RefreshToken(It.IsAny<NukiAccountExternalRequest>()))
            .Returns(async () => await  Task.FromResult(Result.Fail(new ExternalServiceUnreachableError())));

        var response = await _interactor.Handle(new AuthenticateRepositoryPresentationRequest
        {
            NukiAccountId = 0
        });

        response.IsFailed.Should().BeTrue();
        _persistent.Verify(c => c.GetAccount(It.IsAny<int>()));
        _nukiAccountClient.Verify(c => 
            c.RefreshToken(It.Is<NukiAccountExternalRequest>(n => n.RefreshToken == "VALID_REFRESH_TOKEN")));
        response.Errors.First().Should().BeEquivalentTo(new ExternalServiceUnreachableError());
    }
    
}