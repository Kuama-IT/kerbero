using Flurl;
using Flurl.Http;
using KerberoWebApi.Clients.Nuki.Response;
using KerberoWebApi.Utils;

namespace KerberoWebApi.Clients.Nuki;

public class NukiVendorClient : IVendorClient
{
  /// <summary>
  /// Used to trigger the OAuth 2 flow against Nuki API
  /// </summary>
  private string? _clientId;

  private HttpClient _httpClient;
  private readonly NukiVendorClientOptions _options;


  public NukiVendorClient(NukiVendorClientOptions options)
  {
    _options = options;
    _httpClient = new HttpClient();
  }

  public void SetClientId(string? value)
  {
    _clientId = value;
  }

  public Task GetSmartLocks()
  {
    throw new NotImplementedException();
  }

  public Task OpenSmartLock()
  {
    throw new NotImplementedException();
  }

  #region OAuth 2

  /// <summary>
  /// Triggers a new OAuth flow. Returns true if the flow is correctly started
  /// </summary>
  /// <returns></returns>
  public async Task<bool> StartAuthFlow()
  {
    try
    {
      var result = await $"{_options.baseUrl}"
        .AppendPathSegment("oauth")
        .AppendPathSegment("authorize")
        .SetQueryParams(new
        {
          response_type = "code",
          client_id = _clientId,
          redirect_uri = _options.redirectUriForCode,
          scope = _options.scopes,
        })
        .GetAsync();

      return result?.StatusCode == 200;
    }
    catch (Exception e)
    {
      Console.WriteLine("\nException Caught!");
      Console.WriteLine("Message :{0} ", e.Message);
      return false;
    }
  }

  /// <summary>
  /// Retrieves an authentication token from the Nuki APIs
  /// </summary>
  /// <param name="code"></param>
  /// <returns></returns>
  public virtual async Task<NukiGetTokenResponse> RetrieveTokens(string code)
  {
    return await $"{_options.baseUrl}"
      .AppendPathSegment("oauth")
      .AppendPathSegment("token")
      .PostJsonAsync(new
      {
        client_id = _clientId,
        client_secret = _options.clientSecret,
        grant_type = "authorization_code",
        code = code,
        redirect_uri = _options.redirectUriForAuthToken,
      })
      .ReceiveJson<NukiGetTokenResponse>();
  }

  #endregion
}