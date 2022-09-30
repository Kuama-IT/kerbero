namespace Kerbero.Infrastructure.NukiAuthentication.Options;

public class NukiExternalOptions
{
	/// <summary>
	/// Used to complete the OAuth 2 flow against Nuki API
	/// </summary>
	public string ClientSecret { get; set; } = null!;

	/// <summary>
	/// Domain for OAuth redirection
	/// </summary>
	public string MainDomain { get; set; } = null!;

	/// <summary>
	/// Where we expect Nuki API to redirect in order to send us the authentication code (used to retrieve authentication token)
	/// </summary>
	public string RedirectUriForCode { get; set; } = null!;

	/// <summary>
	/// Where we expect Nuki API to redirect in order to send us the final authentication token
	/// </summary>
	public string RedirectUriForAuthToken { get; set; } = null!;

	/// <summary>
	/// List of scopes we will ask to Nuki API
	/// </summary>
	public string Scopes { get; set; } = null!;
  
	/// <summary>
	/// Base url of the Nuki API
	/// </summary>
	public string BaseUrl { get; set; } = null!;
}