using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace KerberoWebApi.Models.Device;

// A device key is a temporary key use to lock/unlock a device
public class DeviceKey
{
  [Key]
  public string Value { get; set; } = null!;
  public DateTime InitialDate { get; set; }
  public DateTime EndDate { get; set; }
  public string? GuestsEmails { get; set; } // email divided by ;
  public DateTime SendingDate { get; set; }
  public KeyStatus state { get; set; } = 0;
  public enum KeyStatus
  {
    Active,
    Disabled,
    Expired
  }
  
  // foreign key
  public int DeviceSmartLockId { get; set; } 
  public DeviceSmartLock DeviceSmartLock { get; set; } = null!;
}