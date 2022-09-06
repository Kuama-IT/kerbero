namespace Kerbero.NET.Domain.Entities.Device;
/// <summary>
/// A device key is a temporary key use to lock/unlock a device
/// </summary>
public class DeviceKey
{
  public string Value { get; set; } = null!;
  
  public DateTime InitialDate { get; set; }
  
  public DateTime? EndDate { get; set; }
  
  public string? GuestsEmails { get; set; } // email divided by ;
  
  public DateTime SendingDate { get; set; }
  
  public KeyStatus State { get; set; } = 0;
  
  public SmartLock SmartLock { get; set; } = null!;
  
  public enum KeyStatus
  {
    Active,
    Disabled,
    Expired
  }
}