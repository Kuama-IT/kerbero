using System.Net;
using FluentAssertions;
using FluentResults;
using Flurl.Http.Testing;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Infrastructure.NukiAuthentication.Options;
using Kerbero.Infrastructure.NukiAuthentication.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kerbero.Infrastructure.Tests.NukiAuthentication.Repositories;

public class NukiAccountExternalRepositoryTests: IDisposable
{
	private readonly NukiAccountExternalRepository _nukiClient;
	private readonly HttpTest _httpTest;
	private readonly Mock<ILogger<NukiAccountExternalRepository>> _logger;
	
	public NukiAccountExternalRepositoryTests()
	{
		// Arrange
		_logger = new Mock<ILogger<NukiAccountExternalRepository>>();
		_nukiClient = new NukiAccountExternalRepository(Microsoft.Extensions.Options.Options.Create(new NukiExternalOptions()
		{
			Scopes = "account notification smartlock smartlock.readOnly smartlock.action smartlock.auth smartlock.config smartlock.log",
			RedirectUriForCode = "/nuki/code",
			MainDomain = "https://test.com",
			BaseUrl = "http://api.nuki.io"
		}), _logger.Object);
		_httpTest = new HttpTest();
	}
	
	public void Dispose()
	{
		_httpTest.Dispose();
		GC.SuppressFinalize(this);
	}
	
	#region BuildUriForCode

	[Fact]
	public void BuildUriForCode_UriForRedirect_Success_Test()
	{
		// Act
		var redirect = _nukiClient.BuildUriForCode(new NukiRedirectExternalRequestDto("v7kn_NX7vQ7VjQdXFGK43g"));

		// Assert
		var equalUri = new Uri("http://api.nuki.io/oauth/authorize?response_type=code" +
		                       "&client_id=v7kn_NX7vQ7VjQdXFGK43g" +
		                       "&redirect_uri=https%3A%2F%2Ftest.com%2Fnuki%2Fcode%2Fv7kn_NX7vQ7VjQdXFGK43g" +
		                       "&scope=account notification smartlock smartlock.readOnly smartlock.action smartlock.auth smartlock.config smartlock.log");
		redirect.Should().BeOfType<Result<NukiRedirectPresentationDto>>();
		redirect.Value.RedirectUri.Should().BeEquivalentTo(equalUri);
	}
	
	[Fact]
	public void BuildUriForCode_ButClientIdIsEmpty_Test()
	{
		// Act
		var exCode = _nukiClient.BuildUriForCode(new NukiRedirectExternalRequestDto(""));
		exCode.IsFailed.Should().BeTrue();
		exCode.Errors.FirstOrDefault().Should().BeOfType<InvalidParametersError>();
	}	
	
	[Fact]
	public void BuildUriForCode_ArgumentNullException_Test()
	{
		// Arrange
		var errorClient = new NukiAccountExternalRepository(Microsoft.Extensions.Options.Options.Create(new NukiExternalOptions()
		{
			Scopes = "account notification smartlock smartlock.readOnly smartlock.action smartlock.auth smartlock.config smartlock.log",
			RedirectUriForCode = null!,
			MainDomain = "https://test.com",
			BaseUrl = "http://api.nuki.io"
		}), _logger.Object);
		
		// Act
		var exCode = errorClient.BuildUriForCode(new NukiRedirectExternalRequestDto("INVALID_CLIENT_ID"));
		// Assert
		exCode.IsFailed.Should().BeTrue();
		exCode.Errors.FirstOrDefault().Should().BeOfType<InvalidParametersError>();
	}	
	
	[Fact]
	public void BuildUriForCode_GenericError_Test()
	{
		// Arrange
		_httpTest.SimulateException(new Exception());
		// Act
		var exCode = _nukiClient.BuildUriForCode(new NukiRedirectExternalRequestDto(""));
		// Assert
		exCode.IsFailed.Should().BeTrue();
		exCode.Errors.FirstOrDefault().Should().BeOfType<InvalidParametersError>();
	}
	#endregion

	#region GetNukiAccount

	[Fact]
	public async void GetNukiAccount_ReturnSuccess_Test()
	{
		//Arrange
		_httpTest.RespondWithJson(new
		{
			access_token ="ACCESS_TOKEN", 
			token_type = "bearer", 
			expires_in = 2592000, 
			refresh_token = "REFRESH_TOKEN"
		}); // from nuki documentation
		
		// Act
		var nukiAccount = await _nukiClient.GetNukiAccount(new NukiAccountExternalRequestDto(){ ClientId = "clientId", Code = "code"});
		
		// Assert
		_httpTest.ShouldHaveMadeACall();
		nukiAccount.Should().BeOfType<Result<NukiAccountExternalResponseDto>>();
		nukiAccount.Value.Should().BeEquivalentTo(new NukiAccountExternalResponseDto()
		{
			Token = "ACCESS_TOKEN",
			RefreshToken = "REFRESH_TOKEN",
			ClientId = "clientId",
			TokenType = "bearer",
			TokenExpiresIn = 2592000
		});
	}

    [Fact]
    public async void GetNukiAccount_ButClientIdIsEmpty_Test()
	{
		// Act
		var exToken = await _nukiClient.GetNukiAccount(new NukiAccountExternalRequestDto() {ClientId = "", Code = ""}) ;
		// Assert
		exToken.IsFailed.Should().BeTrue();
		exToken.Errors.FirstOrDefault().Should().BeOfType<InvalidParametersError>();
	}

    [Fact]
    public async void GetNukiAccount_ButNukiReturnsInvalidParameterError_Test()
    {
	    //Arrange
	    _httpTest.RespondWith(status: 401, body: System.Text.Json.JsonSerializer.Serialize(new 
	    {
		    error_description = "Invalid client credentials.",
		    error = "invalid_client"
	    })); 

	    // Act
	    // Assert
	    var ex = await _nukiClient.GetNukiAccount(new NukiAccountExternalRequestDto() {ClientId = "clientId", Code = "code"});
	    ex.IsFailed.Should().BeTrue();
	    ex.Errors.FirstOrDefault().Should().BeOfType<InvalidParametersError>();
	    ex.Errors.FirstOrDefault()!.Message.Should().Contain("invalid_client: Invalid client credentials.");
    }

    [Fact]
    public async void GetNukiAccount_ButNukiReturnsServerOrTimeoutError_Test()
    {
	    //Arrange
	    _httpTest.RespondWith(status: (int)HttpStatusCode.RequestTimeout, body: System.Text.Json.JsonSerializer.Serialize(new { })); 

	    // Act
	    var ex = await _nukiClient.GetNukiAccount(new NukiAccountExternalRequestDto() {ClientId = "clientId", Code = "code"});

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
	    var ex = await _nukiClient.GetNukiAccount(new NukiAccountExternalRequestDto() {ClientId = "clientId", Code = "code"});
	    ex.IsFailed.Should().BeTrue();
	    ex.Errors.FirstOrDefault().Should().BeOfType<UnknownExternalError>();
    }
    
    [Fact]
    public async void GetAuthenticatedProvider_ButNukiReturnsNull_Test()
    {
	    //Arrange
	    _httpTest.RespondWith(status: 200, body: null); 

	    // Act
	    var ex = await _nukiClient.GetNukiAccount(new NukiAccountExternalRequestDto() {ClientId = "clientId", Code = "code"});
	    // Assert
	    ex.Errors.FirstOrDefault()!.Should().BeOfType<UnableToParseResponseError>();
	    ex.Errors.FirstOrDefault()!.Message.Should().Contain("Response is null");
    }

	#endregion

	#region RefreshToken

	[Fact]
	public async void RefreshToken_ReturnSuccess_Test()
	{
		//Arrange
		_httpTest.RespondWithJson(new
		{
			access_token ="ACCESS_TOKEN", 
			token_type = "bearer", 
			expires_in = 2592000, 
			refresh_token = "REFRESH_TOKEN"
		}); // from nuki documentation
		
		// Act
		var nukiAccount = await _nukiClient.RefreshToken(
			new NukiAccountExternalRequestDto{ ClientId = "clientId", RefreshToken = "VALID_REFRESH_TOKEN"});
		
		// Assert
		_httpTest.ShouldHaveMadeACall();
		nukiAccount.Should().BeOfType<Result<NukiAccountExternalResponseDto>>();
		nukiAccount.Value.Should().BeEquivalentTo(new NukiAccountExternalResponseDto()
		{
			Token = "ACCESS_TOKEN",
			RefreshToken = "REFRESH_TOKEN",
			ClientId = "clientId",
			TokenType = "bearer",
			TokenExpiresIn = 2592000
		});
	}

    [Fact]
    public async void RefreshToken_ButClientIdIsEmpty_Test()
	{
		// Act
		var exToken = await _nukiClient.RefreshToken(new NukiAccountExternalRequestDto() {ClientId = "", Code = ""}) ;
		// Assert
		exToken.IsFailed.Should().BeTrue();
		exToken.Errors.FirstOrDefault().Should().BeOfType<InvalidParametersError>();
	}

    #endregion
}

