using Flurl;
using Flurl.Http;
using System.Text.Json;
using KerberoWebApi.Clients.Nuki.Responses;
using KerberoWebApi.Clients.Requests;
using KerberoWebApi.Clients.Responses;
using KerberoWebApi.Models.Device;

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
        string? response = await $"{_options.BaseUrl}"
                .AppendPathSegments("smartlock")
                .WithOAuthBearerToken(bearerToken)
                .GetStringAsync();
         responseList = JsonSerializer.Deserialize<List<NukiSmartLockResponse>>(response) ?? responseList;
          // ?? throw("Can not parse the response from GetSmartLocks (nuki): " + response.ToString());
    }
    return responseList.Cast<ISmartLockResponse>().ToList();
  }
  
  public async Task<ISmartLockResponse> GetSmartLock(int smartLockId)
  {
    NukiSmartLockResponse response= new NukiSmartLockResponse();
    if(!String.IsNullOrWhiteSpace(bearerToken))
    {
      string? apiResponse = await $"{_options.BaseUrl}"
        .AppendPathSegments("smartlock", smartLockId.ToString())
        .WithOAuthBearerToken(bearerToken)
        .GetStringAsync();
      response = JsonSerializer.Deserialize<NukiSmartLockResponse>(apiResponse) ?? response;
      // ?? throw("Can not parse the response from GetSmartLocks (nuki): " + response.ToString());
    }
    return response;
  }

  public DeviceSmartLock MapSmartLockDeviceRequest(SmartLockRequest request, DeviceVendorAccount account)
  {
    return new DeviceSmartLock()
    {
      VendorSmartlockId = request.smartlockId,
      Status = request.state is null ? null : ((SmartLockRequest.SmartLockState)(request.state.state)).ToString(),
      LastAction = request.state is null ? null : ((SmartLockRequest.LastAction)(request.state.lastAction)).ToString(),
      DeviceVendorAccount = account,
    };
  }
  
  public DeviceSmartLock MapSmartLockDeviceResponse(ISmartLockResponse request, DeviceVendorAccount account)
  {
    return new DeviceSmartLock()
    {
      VendorSmartlockId = request.smartlockId,
      Status = ((NukiSmartLockResponse.SmartLockState)(request.state.state)).ToString(),
      LastAction = ((NukiSmartLockResponse.LastAction)(request.state.lastAction)).ToString(),
      DeviceVendorAccount = account,
    };
  }

  public async Task<bool> OpenSmartLock(int smartLockId)
  {
    bool response = false;
    if(!String.IsNullOrWhiteSpace(bearerToken))
    {
      var apiResponse = await $"{_options.BaseUrl}"
        .AppendPathSegments("smartlock", smartLockId.ToString(), "action", "unlock")
        .WithOAuthBearerToken(bearerToken)
        .PostAsync();
      response = apiResponse.ResponseMessage.IsSuccessStatusCode;
      // ?? throw("Can not parse the response from GetSmartLocks (nuki): " + response.ToString());
    }
    return response;
  }
  
  public async Task<bool> CloseSmartLock(int smartLockId)
  {
    bool response = false;
    if(!String.IsNullOrWhiteSpace(bearerToken))
    {
      var apiResponse = await $"{_options.BaseUrl}"
        .AppendPathSegments("smartlock", smartLockId.ToString(), "action", "lock")
        .WithOAuthBearerToken(bearerToken)
        .PostAsync();
      response = apiResponse.ResponseMessage.IsSuccessStatusCode;
      // ?? throw("Can not parse the response from GetSmartLocks (nuki): " + response.ToString());
    }
    return response;
  }
}



