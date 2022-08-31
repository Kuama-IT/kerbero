using System.Security.Cryptography;
using System.Text;

namespace KerberoWebApi.Models.Device;

// A device key is a temporary key use to lock/unlock a device
public class DeviceKey
{
  public string Value { get; set; }
  public DateTime InitialDate { get; set; }
  public DateTime EndDate { get; set; }
  public List<string>? GuestsEmails { get; set; }
  public DateTime SendingDate { get; set; }
  public Device Device { get; set; }

  public DeviceKey(DateTime initialDate, DateTime endDate, DateTime sendingDate, ref Device device)
  {
    Value = GenerateNewKeyValue(initialDate.ToString(), endDate.ToString(), device.VendorInfo.Name);
    InitialDate = initialDate;
    EndDate = endDate;
    SendingDate = sendingDate;
    Device = device;
  }

  // temporary method to give an idea of the flow
  private string GenerateNewKeyValue(string info1, string info2, string info3)
  {
    Random rand = new Random();
    string value = info1 + info2 + info3 + rand.Next().ToString();
    StringBuilder Sb = new StringBuilder();

    using (SHA256 hash = SHA256Managed.Create())
    {
      Encoding enc = Encoding.UTF8;
      Byte[] result = hash.ComputeHash(enc.GetBytes(value));

      foreach (Byte b in result)
        Sb.Append(b.ToString("x2"));
    }

    return Sb.ToString();
  }

  public enum KeyStatus
  {
    Active,
    Disabled,
    Expired
  }
}