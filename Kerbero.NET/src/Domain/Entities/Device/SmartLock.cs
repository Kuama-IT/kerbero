using Kerbero.NET.Domain.Entities.Account;
using Kerbero.NET.Domain.Entities.RentInformation;

namespace Kerbero.NET.Domain.Entities.Device;

/// <summary>
/// A SmartLock is a logic entity which represents the physical smart-lock
/// </summary>
public class SmartLock
{
  public int Id { get; set; }
  
  public int VendorSmartLockId { get; set; }
  
  public string? Model { get; set; } 
  
  public string? Image { get; set; }
  
  public Status? State { get; set; }
  
  public int? UnlockNumbers { get; set; }
  
  public string? LastAction { get; set; }
  
  public List<DeviceKey>? DeviceKeys { get; set; }
  
  public List<SmartLockLog>? DeviceLogs { get; set; }

  public List<Reservation>? Reservations { get; set; } = null!;

  public SmartLockProviderAccount SmartLockProviderAccount { get; set; } = null!;
  
  public enum Status
  {
      Open,
      Close,
      Active,
      Unreachable,
      Undefined
  }
}