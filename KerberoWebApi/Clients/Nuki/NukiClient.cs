using Flurl;
using Flurl.Http;
using System.Text.Json;
using KerberoWebApi.Clients.Nuki.Response;
using KerberoWebApi.Clients.IResponse;

namespace KerberoWebApi.Clients.Nuki;

public class NukiClient: IVendorClient
{
    private string? bearerToken;
    private readonly NukiVendorClientOptions _options;
    public string Name { get; }

    public NukiClient(NukiVendorClientOptions options)
    {
        _options = options;
        Name = "nuki";
    } 

    public void SetToken(string bearer)
    {
      bearerToken = bearer;
    }


  public async Task<List<ISmartLockResponse>> GetSmartLocks()
  {
    List<NukiSmartLockResponse> responseList = new List<NukiSmartLockResponse>();
    if(!String.IsNullOrWhiteSpace(bearerToken))
    {
        string? response = await $"{_options.baseUrl}"
                .AppendPathSegments("smartlock")
                .WithOAuthBearerToken(bearerToken)
                .GetStringAsync();
         responseList = JsonSerializer.Deserialize<List<NukiSmartLockResponse>>(response) ?? responseList;
          // ?? throw("Can not parse the response from GetSmartLocks (nuki): " + response.ToString());
    }
    return responseList.Cast<ISmartLockResponse>().ToList();
;
  }

  public Task OpenSmartLock()
  {
    throw new NotImplementedException();
  }

  Task IVendorClient.CloseSmartLock()
  {
      throw new NotImplementedException();
  }
}



