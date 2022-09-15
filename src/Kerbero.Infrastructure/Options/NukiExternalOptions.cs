namespace Kerbero.Infrastructure.Options;

public class NukiExternalOptions
{
	/// <summary>
	/// String identifier for the appsettings.json
	/// </summary>
	public string NukiClient = "NukiClient";
	
	/// <summary>
	/// Used to complete the OAuth 2 flow against Nuki API
	/// </summary>
	public string ClientSecret = null!;

	/// <summary>
	/// Domain for OAuth redirection
	/// </summary>
	public string MainDomain = null!;

	/// <summary>
	/// Where we expect Nuki API to redirect in order to send us the authentication code (used to retrieve authentication token)
	/// </summary>
	public string RedirectUriForCode = null!;

	/// <summary>
	/// Where we expect Nuki API to redirect in order to send us the final authentication token
	/// </summary>
	public string RedirectUriForAuthToken = null!;

	/// <summary>
	/// List of scopes we will ask to Nuki API
	/// </summary>
	public string Scopes = null!;
  
	/// <summary>
	/// Base url of the Nuki API
	/// </summary>
	public string BaseUrl = null!;
}