// Basic interface for vendor api clients, used to communicate with vendor's Smart locks.
using KerberoWebApi.Clients.Responses;

namespace KerberoWebApi.Clients;

/// <summary>
/// Interface that should be implemented by all the new Client for a specific vendor.
/// The common controller used that interface to perform the API client calls.
/// </summary>
public interface IVendorClient
{
  public Task<SmartlockListResponses> GetSmartLocks();

  public Task OpenSmartLock();

  public Task CloseSmartLock();
}