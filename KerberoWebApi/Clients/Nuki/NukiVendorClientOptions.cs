namespace KerberoWebApi.Clients.Nuki;

public class NukiVendorClientOptions
{

  /// <summary>
  /// Used to complete the OAuth 2 flow against Nuki API
  /// </summary>
  public readonly string ClientSecret;

  /// <summary>
  /// Domain for OAuth redirection
  /// </summary>
  public readonly string MainDomain;

  /// <summary>
  /// Where we expect Nuki API to redirect in order to send us the authentication code (used to retrieve authentication token)
  /// </summary>
  public readonly string RedirectUriForCode;

  /// <summary>
  /// Where we expect Nuki API to redirect in order to send us the final authentication token
  /// </summary>
  public readonly string RedirectUriForAuthToken;

  /// <summary>
  /// List of scopes we will ask to Nuki API
  /// </summary>
  public readonly string Scopes;
  
  /// <summary>
  /// Base url of the Nuki API
  /// </summary>
  public readonly string BaseUrl;

  public NukiVendorClientOptions(string clientSecret, string mainDomain, string redirectUriForCode, string redirectUriForAuthToken, string scopes, string baseUrl)
  {
    this.ClientSecret = clientSecret;
    this.MainDomain = mainDomain;
    this.RedirectUriForCode = redirectUriForCode;
    this.RedirectUriForAuthToken = redirectUriForAuthToken;
    this.Scopes = scopes;
    this.BaseUrl = baseUrl;
  }
}