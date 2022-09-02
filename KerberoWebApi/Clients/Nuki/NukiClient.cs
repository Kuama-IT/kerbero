using KerberoWebApi.Clients.Responses;
namespace KerberoWebApi.Clients;

public class NukiClient: IVendorClient
{

  public async Task<SmartlockListResponses> GetSmartLocks()
  {
    var list = new SmartlockListResponses();
    // TODO arrange models to not be dependent on DeviceVendorAccount
    list.Append(new Models.Device.Device());
    return list;
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



