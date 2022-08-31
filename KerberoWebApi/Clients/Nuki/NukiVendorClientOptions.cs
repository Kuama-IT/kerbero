namespace KerberoWebApi.Clients.Nuki;

public class NukiVendorClientOptions
{

  /// <summary>
  /// Used to complete the OAuth 2 flow against Nuki API
  /// </summary>
  public readonly string clientSecret;

  /// <summary>
  /// Where we expect Nuki API to redirect in order to send us the authentication code (used to retrieve authentication token)
  /// </summary>
  public readonly string redirectUriForCode;

  /// <summary>
  /// Where we expect Nuki API to redirect in order to send us the final authentication token
  /// </summary>
  public readonly string redirectUriForAuthToken;

  /// <summary>
  /// List of scopes we will ask to Nuki API
  /// </summary>
  public readonly string scopes;

  public readonly string baseUrl;

  public NukiVendorClientOptions(string clientSecret, string redirectUriForCode, string redirectUriForAuthToken, string scopes, string baseUrl)
  {
    this.clientSecret = clientSecret;
    this.redirectUriForCode = redirectUriForCode;
    this.redirectUriForAuthToken = redirectUriForAuthToken;
    this.scopes = scopes;
    this.baseUrl = baseUrl;
  }
}