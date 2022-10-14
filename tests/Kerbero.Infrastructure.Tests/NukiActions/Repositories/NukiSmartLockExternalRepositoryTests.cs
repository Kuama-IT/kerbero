using FluentAssertions;
using Flurl.Http.Testing;
using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiAuthentication.Entities;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Infrastructure.NukiActions;
using Kerbero.Infrastructure.NukiAuthentication.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Kerbero.Infrastructure.Tests.NukiActions.Repositories;

public class NukiSmartLockExternalRepositoryTests: IDisposable
{
    private readonly NukiSmartLockExternalRepository _nukiSmartLockClient;
    private readonly HttpTest _httpTest;

    public NukiSmartLockExternalRepositoryTests()
    {
        // Arrange
        var logger = new Mock<ILogger<NukiSmartLockExternalRepository>>();
        _nukiSmartLockClient = new NukiSmartLockExternalRepository(Options.Create(new NukiExternalOptions()
        {
            Scopes = "account notification smartlock smartlock.readOnly smartlock.action smartlock.auth smartlock.config smartlock.log",
            RedirectUriForCode = "/nuki/code",
            MainDomain = "https://test.com",
            BaseUrl = "http://api.nuki.io"
        }), logger.Object);
        _httpTest = new HttpTest();
    }
	
    public void Dispose()
    {
        _httpTest.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void AuthenticateRepository_Success_Test()
    {
        // Act
        _nukiSmartLockClient.Authenticate(new NukiAccount
        {
            Id = 1,
            Token = "ACCESS_TOKEN",
            RefreshToken = "REFRESH_TOKEN",
            ClientId = "clientId",
            TokenType = "bearer",
        });
        
        // Assert
        _nukiSmartLockClient.IsAuthenticated.Should().BeTrue();
    }
}