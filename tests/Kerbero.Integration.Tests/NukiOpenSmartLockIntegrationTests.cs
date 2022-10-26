using System.Net;
using FluentAssertions;
using Flurl.Http.Testing;
using Kerbero.WebApi;
using Microsoft.Extensions.Configuration;

namespace Kerbero.Integration.Tests;

public class NukiOpenSmartLockIntegrationTests: IDisposable
{
    private readonly HttpTest _httpTest;
    private readonly KerberoWebApplicationFactory<Program> _application;
    private static readonly IConfigurationRoot Config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.Test.json")
        .AddEnvironmentVariables()
        .Build();

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
        await _application.CreateNukiAccount(IntegrationTestsUtils.GetSeedingNukiAccount());
        await _application.CreateNukiSmartLock(IntegrationTestsUtils.GetSeedingNukiSmartLock());
        var client = _application.CreateClient();

        // Act
        var response = await client.PutAsync("api/nuki/smartlock/1/unlock?accountId=1", null);
        
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task OpenSmartLock_NotAuthorized_Test()
    {
        // Arrange
        var client = _application.CreateClient();

        // Act
        var response = await client.PutAsync("api/nuki/smartlock/1/unlock?accountId=0", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task OpenSmartLock_WrongSmartLockId_Test()
    {
        // Arrange
        var client = _application.CreateClient();

        // Act
        var response = await client.PutAsync($"api/nuki/smartlock/{0}/unlock?accountId=1", null);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
}