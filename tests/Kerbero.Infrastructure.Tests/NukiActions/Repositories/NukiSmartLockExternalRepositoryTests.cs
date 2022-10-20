using System.Net;
using FluentAssertions;
using Flurl.Http.Testing;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiActions.Models.ExternalResponses;
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
	public async Task GetNukiSmartLockList_Success_Test()
	{
		// Arrange
		_httpTest.RespondWithJson(
			new[]
			{
				new
				{
					smartlockId = 0,
					accountId = 0,
					type = 0,
					lmType = 0,
					authId = 0,
					name = "string",
					favorite = true,
					config = new
					{
						name = "string",
						latitude = 0,
						longitude = 0,
						capabilities = 2,
						autoUnlatch = true,
						liftUpHandle = true,
						pairingEnabled = true,
						buttonEnabled = true,
						ledEnabled = true,
						ledBrightness = 0,
						timezoneOffset = 0,
						daylightSavingMode = 0,
						fobPaired = true,
						fobAction1 = 8,
						fobAction2 = 8,
						fobAction3 = 8,
						singleLock = true,
						operatingMode = 0,
						advertisingMode = 3,
						keypadPaired = true,
						keypad2Paired = true,
						homekitState = 3,
						timezoneId = 45,
						deviceType = 0,
						wifiEnabled = true,
						operationId = "string"
					},
					advancedConfig = new
					{
						lngTimeout = 5,
						singleButtonPressAction = 0,
						doubleButtonPressAction = 0,
						automaticBatteryTypeDetection = true,
						unlatchDuration = 1,
						operationId = "string",
						totalDegrees = 0,
						singleLockedPositionOffsetDegrees = 0,
						unlockedToLockedTransitionOffsetDegrees = 0,
						unlockedPositionOffsetDegrees = 0,
						lockedPositionOffsetDegrees = 0,
						detachedCylinder = true,
						batteryType = 0,
						autoLock = true,
						autoLockTimeout = 0,
						autoUpdateEnabled = true
					},
					openerAdvancedConfig = new
					{
						intercomId = 0,
						busModeSwitch = 0,
						shortCircuitDuration = 0,
						electricStrikeDelay = 0,
						randomElectricStrikeDelay = true,
						electricStrikeDuration = 0,
						disableRtoAfterRing = true,
						rtoTimeout = 0,
						doorbellSuppression = 0,
						doorbellSuppressionDuration = 0,
						soundRing = 0,
						soundOpen = 0,
						soundRto = 0,
						soundCm = 0,
						soundConfirmation = 0,
						soundLevel = 0,
						singleButtonPressAction = 0,
						doubleButtonPressAction = 0,
						batteryType = 0,
						automaticBatteryTypeDetection = true,
						autoUpdateEnabled = true,
						operationId = "string"
					},
					smartdoorAdvancedConfig = new
					{
						lngTimeout = 5,
						singleButtonPressAction = 0,
						doubleButtonPressAction = 0,
						automaticBatteryTypeDetection = true,
						unlatchDuration = 1,
						operationId = "string",
						buzzerVolume = 0,
						supportedBatteryTypes = new[] { 0 },
						batteryType = 0,
						autoLockTimeout = 0,
						autoLock = true
					},
					webConfig = new
					{
						batteryWarningPerMailEnabled = true,
						dismissedLiftUpHandleWarning = new[] { 0 }
					},
					state = new
					{
						mode = 4,
						state = 255,
						trigger = 6,
						lastAction = 5,
						batteryCritical = true,
						batteryCharging = true,
						batteryCharge = 100,
						keypadBatteryCritical = true,
						doorsensorBatteryCritical = true,
						doorState = 255,
						ringToOpenTimer = 65535,
						ringToOpenEnd = "2022-10-04T08=10=52.765Z",
						nightMode = true,
						operationId = "string"
					},
					firmwareVersion = 0,
					hardwareVersion = 0,
					operationId = "string",
					serverState = 4,
					adminPinState = 2,
					virtualDevice = true,
					creationDate = "2022-10-04T08:10:52.765Z",
					updateDate = "2022-10-04T08=10:52.765Z",
					subscriptions = new[]
					{
						new
						{
							type = "B2C",
							state = "ACTIVE",
							updateDate = "2022-10-04T08:10:52.765Z",
							creationDate = "2022-10-04T08:10:52.765Z"
						}
					},
					opener = true,
					box = true,
					smartDoor = true,
					keyturner = true,
					smartlock3 = true
				}

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