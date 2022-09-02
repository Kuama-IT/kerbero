using System.Security.Cryptography;
using System.Text;

namespace KerberoWebApi.Models.Device;

// A device key is a temporary key use to lock/unlock a device
public class DeviceKey
{
  public string Value { get; set; } = null!;
  public DateTime InitialDate { get; set; }
  public DateTime EndDate { get; set; }
  public List<string>? GuestsEmails { get; set; }
  public DateTime SendingDate { get; set; }
  public Device Device { get; set; } = null!;
  public enum KeyStatus
  {
    Active,
    Disabled,
    Expired
  }
}