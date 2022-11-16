using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Flurl.Http.Testing;
using Kerbero.Identity.Library.Modules.Authentication.Dtos;
using Kerbero.WebApi;
using Kerbero.WebApi.Models.Requests;

namespace Kerbero.Integration.Tests.SmartLocks;

public class NukiOpenSmartLockIntegrationTests: IDisposable
{
    private readonly HttpTest _httpTest;
    private readonly KerberoWebApplicationFactory<Program> _application;

    public NukiOpenSmartLockIntegrationTests()
    {
        _application = new KerberoWebApplicationFactory<Program>();
        _application.Server.PreserveExecutionContext = true; // fixture for Flurl
        _httpTest = new HttpTest();
    }
    
    	
    public void Dispose()
    {
        _httpTest.Dispose();
    }

    [Fact]
    public async Task OpenSmartLock_Success_Test()
    {
        // Arrange
        var client = await _application.GetLoggedClient();
        await _application.CreateNukiAccount(IntegrationTestsUtils.GetSeedingNukiAccount());
        await _application.CreateNukiSmartLock(IntegrationTestsUtils.GetSeedingNukiSmartLock());

        // Act
        var response = await client.PutAsJsonAsync("api/smartlocks/unlock",
            new OpenNukiSmartLockRequest(1, 1));
        
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task OpenSmartLock_NotAuthorized_Test()
    {
        // Arrange
        _application.ClientOptions.HandleCookies = true;
        var client = await _application.GetLoggedClient();

        // Act
        var response = await client.PutAsJsonAsync("api/smartlocks/unlock",
            new OpenNukiSmartLockRequest(0, 1));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task OpenSmartLock_WrongSmartLockId_Test()
    {
        // Arrange
        await _application.CreateNukiAccount(IntegrationTestsUtils.GetSeedingNukiAccount());
        var client = await _application.GetLoggedClient();
        
        // Act
        var response = await client.PutAsJsonAsync("api/smartlocks/unlock",
            new OpenNukiSmartLockRequest(1, 0));
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
}
