// Basic interface for vendor api clients, used to communicate with vendor's Smart locks.
using KerberoWebApi.Clients.IResponse;

namespace KerberoWebApi.Clients;

/// <summary>
/// Interface that should be implemented by all the new Client for a specific vendor.
/// The common controller used that interface to perform the API client calls.
/// </summary>
public interface IVendorClient
{
  public string Name { get; }

  public abstract void SetToken(string bearer);

  public Task<List<ISmartLockResponse>> GetSmartLocks();

  public Task OpenSmartLock();

  public Task CloseSmartLock();
}
