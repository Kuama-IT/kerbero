using FluentAssertions;
using Flurl.Http.Testing;
using Kerbero.Common.Entities;
using Kerbero.Common.Exceptions;
using Kerbero.Infrastructure.Clients;
using Kerbero.Infrastructure.Options;

namespace Kerbero.Infrastructure.Tests.Clients;

public class NukiHttpClientTests: IDisposable
{
	private readonly NukiHttpClient _nukiClient;
	private readonly HttpTest _httpTest;

	public NukiHttpClientTests()
	{
		// Arrange
		_nukiClient = new NukiHttpClient(Microsoft.Extensions.Options.Options.Create(new NukiClientOptions()
		{
			Scopes = "account notification smartlock smartlock.readOnly smartlock.action smartlock.auth smartlock.config smartlock.log",
			RedirectUriForCode = "/nuki/code",
			MainDomain = "https://test.com",
			BaseUrl = "https://api.nuki.io"
		}));
		_httpTest = new HttpTest();
	}
	
	public void Dispose()
	{
		_httpTest.Dispose();
	}

	[Fact]
	public void AskCodeAndReceiveUriForRedirect_Success_Test()
	{
		// Act
		var redirect = _nukiClient.BuildUriForCode("v7kn_NX7vQ7VjQdXFGK43g");

		// Assert
		var equalUri = new Uri("http://api.nuki.io/oauth/authorize?response_type=code" +
		                       "&client_id=v7kn_NX7vQ7VjQdXFGK43g" +
		                       "&redirect_uri=https://test.com/nuki/code/v7kn_NX7vQ7VjQdXFGK43g" +
		                       "&scope=account notification smartlock smartlock.readOnly smartlock.action smartlock.auth smartlock.config smartlock.log");
		Assert.Equal(redirect.AbsolutePath, equalUri.AbsolutePath);
	}
	


	[Fact]
	public async void RetrieveTokensFromAuthenticationCode_Success_Test()
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
		var nukiAccount = await _nukiClient.GetAuthenticatedProvider("clientId", "code");
		
		// Assert
		_httpTest.ShouldHaveMadeACall();
		nukiAccount.Should().BeEquivalentTo(new NukiAccountEntity()
		{
			Token = "ACCESS_TOKEN",
			RefreshToken = "REFRESH_TOKEN",
			ClientId = "clientId",
			TokenType = "bearer",
			TokenExpiresIn = 2592000
		});
	}
}

public class NukiHttpClientTestsExceptions : IDisposable
{
	private readonly NukiHttpClient _nukiClient;
	private readonly HttpTest _httpTest;

	public NukiHttpClientTestsExceptions()
	{
		// Arrange
		_nukiClient = new NukiHttpClient(Microsoft.Extensions.Options.Options.Create(new NukiClientOptions()
		{
			Scopes =
				"account notification smartlock smartlock.readOnly smartlock.action smartlock.auth smartlock.config smartlock.log",
			RedirectUriForCode = "/nuki/code",
			MainDomain = "https://test.com",
			BaseUrl = "https://api.nuki.io"
		}));
		_httpTest = new HttpTest();
	}

	public void Dispose()
	{
		_httpTest.Dispose();
	}
	
	[Fact]
	public void AskToAuthenticateToTheClient_ButClientIdIsEmpty_Test()
	{
		// Assert
		Exception exCode = Assert.Throws<InvalidClientIdException>(() => 
			_nukiClient.BuildUriForCode("") );
		Assert.Matches("EmptyOrNull, Exception of type 'Kerbero.Common.Exceptions.InvalidClientIdException' was thrown.", exCode.Message);
	}

    [Fact]
    public async void RetrieveAuthenticateToTheClient_ButClientIdIsEmpty_Test()
	{
		// Assert
		Exception exToken = await Assert.ThrowsAsync<InvalidClientIdException>(async () => 
			await _nukiClient.GetAuthenticatedProvider(" ", " ") );
		Assert.Matches("EmptyOrNull, Exception of type 'Kerbero.Common.Exceptions.InvalidClientIdException' was thrown.", exToken.Message);
	}

    [Fact]
    public async void GetAuthenticatedProvider_ButNukiReturnsError_Test()
    {
	    //Arrange
	    _httpTest.RespondWithJson(new
	    {
		    expires_in = 2592000, 
		    refresh_token = "REFRESH_TOKEN"
	    }); 
		
	    // Act
	    // Assert
	    Exception exToken = await Assert.ThrowsAsync<InconsistentApiResponseException>(async () =>
		    await _nukiClient.GetAuthenticatedProvider("clientId", "code"));
	    Assert.Matches("{   \"expires_in\": 2592000,   \"refresh_token\": \"REFRESH_TOKEN\" }, AttributeNotFound", exToken.Message);
    }
}

