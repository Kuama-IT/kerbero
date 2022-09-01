using KerberoWebApi.Models.Device;

namespace KerberoWebApi.Clients.Responses;

public class SmartlockListResponses
{
    public List<Device> SmartlockList {get; private set;}

    public void Append(Device smartLock) {}

    public void Append(ref SmartlockListResponses smartLockList) {}
}