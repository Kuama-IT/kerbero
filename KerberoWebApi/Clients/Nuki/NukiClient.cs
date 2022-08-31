namespace KerberoWebApi.Clients;

public class NukiClient: IVendorClient
{

  public Task GetSmartLocks()
  {
    throw new NotImplementedException();
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



