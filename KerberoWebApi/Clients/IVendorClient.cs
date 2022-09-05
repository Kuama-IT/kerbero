// Basic interface for vendor api clients, used to communicate with vendor's Smart locks.

using KerberoWebApi.Clients.Requests;
using KerberoWebApi.Clients.Responses;
using KerberoWebApi.Models.Device;

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

  public Task<bool> OpenSmartLock(int smartLockVendorSmartlockId);

  public Task<bool> CloseSmartLock(int smartLockVendorSmartlockId);
    
  public DeviceSmartLock MapSmartLockDeviceRequest(SmartLockRequest request, DeviceVendorAccount accountId);
  public DeviceSmartLock MapSmartLockDeviceResponse(ISmartLockResponse res, DeviceVendorAccount account);
  public Task<ISmartLockResponse> GetSmartLock(int deviceVendorSmartlockId);
}
