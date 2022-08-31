using System.Net.Http.Headers;
using KerberoWebApi.Utils;
using System = global::System;

// Client for the Nuki Api
namespace KerberoWebApi.Clients;

public class NukiApiClientTryOne
{
  private string _baseUrl = "api.nuki.io";
  private HttpClient _httpClient;

  public NukiApiClientTryOne(string accessToken)
  {
    _httpClient = new HttpClient();
    if (String.IsNullOrEmpty(accessToken))
    {
      throw new BadHttpRequestException("Access to Nuki API is not possible without authentication", 400);
    }
    else
      _httpClient.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", accessToken);

    _httpClient.DefaultRequestHeaders.Accept.Clear();
    _httpClient.DefaultRequestHeaders.Accept.Add(
      new MediaTypeWithQualityHeaderValue("application/json"));
    // _settings = new System.Lazy<Newtonsoft.Json.JsonSerializerSettings>(CreateSerializerSettings);
  }

  public string BaseUrl
  {
    get { return _baseUrl; }
    set { _baseUrl = value; }
  }


  // Base method for Nuki GET calls
  protected virtual async Task<string> GetAsyncBase(string path)
  {
    var url = new UrlBuilder().UrlBaseBuilder(_baseUrl, path);
    string responseBody = "";
    try
    {
      HttpResponseMessage response = await _httpClient.GetAsync(url);
      response.EnsureSuccessStatusCode();
      responseBody = await response.Content.ReadAsStringAsync();

      Console.WriteLine(responseBody);
    }
    catch (HttpRequestException e)
    {
      Console.WriteLine("\nException Caught!");
      Console.WriteLine("Message :{0} ", e.Message);
    }

    return responseBody;
  }

  public virtual async Task<string> SmartlocksList()
  {
    return await GetAsyncBase("/smartlock");
  }

  public virtual async Task<bool> AuthorizeApi(string clientSecret)
  {
    var query = new Dictionary<string, string?>();
    query["response_type"] = "code";
    query["client_id"] = clientSecret;
    query["redirect_uri"] = "https://test.com:5220/VendorAuthorization/auth";
    query["scope"] =
      $"account notification smartlock smartlock.readOnly smartlock.action smartlock.auth smartlock.config smartlock.log";
    // use a url builder
    UrlBuilder urlBuilder = new UrlBuilder();
    urlBuilder.Scheme = "https";
    var url = urlBuilder.UrlBaseBuilder(_baseUrl, "/oauth/authorize", query);
    var unAuthClient = new HttpClient();
    try
    {
      HttpResponseMessage response = await _httpClient.GetAsync(url);
      response.EnsureSuccessStatusCode();
    }
    catch (HttpRequestException e)
    {
      Console.WriteLine("\nException Caught!");
      Console.WriteLine("Message :{0} ", e.Message);
      return false;
    }

    return true;
  }

  public virtual async Task<List<string>> RetrieveTokens(string clientId, string clientSecret, string code)
  {
    var returnTokens = new List<string>();
    var responseBody = "";
    var url = "";
    try
    {
      HttpResponseMessage response = await _httpClient.GetAsync(url);
      response.EnsureSuccessStatusCode();
      responseBody = await response.Content.ReadAsStringAsync();
    }
    catch (HttpRequestException e)
    {
      Console.WriteLine("\nException Caught!");
      Console.WriteLine("Message :{0} ", e.Message);
    }

    return returnTokens;
  }
}