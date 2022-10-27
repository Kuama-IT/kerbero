using System.Net;
using System.Text.Json;
using FluentAssertions;
using Flurl.Http.Testing;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiActions.Models.ExternalResponses;
using Kerbero.Infrastructure.Common.Helpers;
using Kerbero.Infrastructure.Common.Options;
using Kerbero.Infrastructure.NukiActions.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Kerbero.Infrastructure.Tests.NukiActions.Repositories;

public class NukiSmartLockExternalRepositoryTests: IDisposable
{
    private readonly NukiSmartLockExternalRepository _nukiSmartLockClient;
    private readonly HttpTest _httpTest;
    private readonly object _nukiJsonSmartlockResponse;

    public NukiSmartLockExternalRepositoryTests()
    {
        // Arrange
        var helper = new NukiSafeHttpCallHelper(new Mock<ILogger<NukiSafeHttpCallHelper>>().Object);
        _nukiSmartLockClient = new NukiSmartLockExternalRepository(Options.Create(new NukiExternalOptions()
        {
            Scopes = "account notification smartlock smartlock.readOnly smartlock.action smartlock.auth smartlock.config smartlock.log",
            RedirectUriForCode = "/nuki/code",
            MainDomain = "https://test.com",
            BaseUrl = "http://api.nuki.io"
        }), helper);
        _httpTest = new HttpTest();
        
        var json = File.ReadAllText("JsonData/get-nuki-smartlock-response.json");
        _nukiJsonSmartlockResponse = JsonSerializer.Deserialize<dynamic>(json) ?? throw new InvalidOperationException();
    }
	
    public void Dispose()
    {
        _httpTest.Dispose();
    }
    
    [Fact]
	public async Task GetNukiSmartLockList_Success_Test()
	{
		// Arrange
		_httpTest.RespondWithJson(
			new[]
			{
				_nukiJsonSmartlockResponse
			});

		// Act
		var response = await _nukiSmartLockClient.GetNukiSmartLocks("ACCESS_TOKEN");

		// Assert
		response.IsSuccess.Should().BeTrue();
		response.Value.Should().BeOfType<List<NukiSmartLockExternalResponse>>();
		response.Value.Should().BeEquivalentTo(new List<NukiSmartLockExternalResponse>
			{
				new()
				{
					AccountId = 0,
					AuthId = 0,
					Favourite = true,
					LmType = 0,
					Name = "string",
					SmartLockId = 0,
					Type = 0,
					State = new NukiSmartLockStateExternalResponse
					{
						BatteryCharge = 100,
						BatteryCharging = true,
						BatteryCritical = true,
						DoorState = 255,
						LastAction = 5,
						Mode = 4,
						OperationId = "string",
						State = 255,
					}
				}
			});
	}

	[Fact]
    public async void GetNukiSmartLockList_ButNukiReturnsInvalidParameterError_Test()
    {
	    //Arrange
	    _httpTest.RespondWith(status: 401, body: System.Text.Json.JsonSerializer.Serialize(new 
	    {
		    error_description = "Invalid client credentials.",
		    error = "invalid_client"
	    })); 

	    // Act
	    // Assert
	    var ex = await _nukiSmartLockClient.GetNukiSmartLocks("ACCESS_TOKEN");
	    ex.IsFailed.Should().BeTrue();
	    ex.Errors.FirstOrDefault().Should().BeOfType<InvalidParametersError>();
	    ex.Errors.FirstOrDefault()!.Message.Should().Contain("invalid_client: Invalid client credentials.");
    }

    [Fact]
    public async void GetNukiSmartLockList_ButNukiReturnsServerOrTimeoutError_Test()
    {
	    //Arrange
	    _httpTest.RespondWith(status: (int)HttpStatusCode.RequestTimeout, body: System.Text.Json.JsonSerializer.Serialize(new { })); 

	    // Act
	    var ex = await _nukiSmartLockClient.GetNukiSmartLocks("ACCESS_TOKEN");

	    // Assert
	    ex.IsFailed.Should().BeTrue();
	    ex.Errors.FirstOrDefault().Should().BeOfType<ExternalServiceUnreachableError>();
    }
    
    [Fact]
    public async void GetAuthenticatedProvider_ButNukiUnknownReturnsError_Test()
    {
	    //Arrange
	    _httpTest.RespondWith(status: 435, body: System.Text.Json.JsonSerializer.Serialize(new 
	    {
		    error_description = "Invalid client credentials.",
		    error = "invalid_client"
	    })); 

	    // Act
	    // Assert
	    var ex = await _nukiSmartLockClient.GetNukiSmartLocks("ACCESS_TOKEN");
	    ex.IsFailed.Should().BeTrue();
	    ex.Errors.FirstOrDefault().Should().BeOfType<UnknownExternalError>();
    }
    
    [Fact]
    public async void GetAuthenticatedProvider_ButNukiReturnsNull_Test()
    {
	    //Arrange
	    _httpTest.RespondWith(status: 200, body: null); 

	    // Act
	    var ex = await _nukiSmartLockClient.GetNukiSmartLocks("ACCESS_TOKEN");
	    // Assert
	    ex.Errors.FirstOrDefault()!.Should().BeOfType<UnableToParseResponseError>();
	    ex.Errors.FirstOrDefault()!.Message.Should().Contain("Response is null");
    }
}