using Flurl;
using Flurl.Http;
using KerberoWebApi.Clients.Nuki.Response;

namespace KerberoWebApi.Clients.Nuki;

public class NukiClientAuthentication
{
    /// <summary>
    /// Used to trigger the OAuth 2 flow against Nuki API
    /// </summary>
    private string? _clientId;

    private HttpClient _httpClient;
    private readonly NukiVendorClientOptions _options;

    public NukiClientAuthentication(NukiVendorClientOptions options)
    {
        _options = options;
        _httpClient = new HttpClient();
    }

    public void SetClientId(string? value)
    {
        _clientId = value;
    }

    /// <summary>
    /// Create the Uri for login Nuki Web page Redirection
    /// </summary>
    /// <returns></returns>
    public Uri? StartAuthFlow()
    {
        try
        {
            var redirect_uri_clientId = $"{_options.redirectUriForCode}".AppendPathSegment(_clientId);
            return $"{_options.baseUrl}"
              .AppendPathSegments("oauth", "authorize")
              .SetQueryParams(new
              {
                  response_type = "code",
                  client_id = _clientId,
                  redirect_uri = redirect_uri_clientId.ToString(),
                  scope = _options.scopes,
              })
              .ToUri();
        }
        catch (Exception e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
            return null;
        }
    }

    /// <summary>
    /// Retrieves an authentication token from the Nuki APIs
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public virtual async Task<NukiGetTokenResponse> RetrieveTokens(string? code)
    {
      if(code != null)
      {
         // uncomment when retrieve the clientsecret
        var redirect_uri_clientId = $"{_options.redirectUriForAuthToken}".AppendPathSegment(_clientId);
        // return await $"{_options.baseUrl}"
        //   .AppendPathSegment("oauth")
        //   .AppendPathSegment("token")
        //   .PostJsonAsync(new
        //   {
        //     client_id = _clientId,
        //     client_secret = _options.clientSecret,
        //     grant_type = "authorization_code",
        //     code = code,
        //     redirect_uri = redirect_uri_clientId,
        //   })
        // .ReceiveJson<NukiGetTokenResponse>();
        // test data
        return new NukiGetTokenResponse()
        {
            AccessToken = _options.clientSecret,
            ExpiresIn = "",
            RefreshToken = "",
            TokenType = "Bearer"
        };
      }
      else
        throw new BadHttpRequestException("No code provided");
    }

    /// <summary>
    /// For now it does nothing.
    /// </summary>
    /// <returns></returns>
    public virtual void SuccessfulCallback()
    {
        throw new NotImplementedException("SuccessfulCallback");
    }
}