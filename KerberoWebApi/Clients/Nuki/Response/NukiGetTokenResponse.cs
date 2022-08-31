namespace KerberoWebApi.Clients.Nuki.Response;

/// <summary>
/// {"access_token":"ACCESS_TOKEN","token_type":"bearer","expires_in":2592000,"refresh_token":"REFRESH_TOKEN"}
/// </summary>
public class NukiGetTokenResponse
{
  public string AccessToken { get; set; }

  public string RefreshToken { get; set; }

  public string TokenType { get; set; }

  public string ExpiresIn { get; set; }
}