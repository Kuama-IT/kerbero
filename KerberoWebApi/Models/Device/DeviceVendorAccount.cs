namespace KerberoWebApi.Models.Device;

// DeviceVendorAccount derive from DeviceVendor class, add the information of an Host account
public class DeviceVendorAccount : DeviceVendor
{
  public string? Id { get; set;}
  public string? Token { get; set; }
  public string? RefreshToken { get; set;}
  public string? ClientId { get; set; }
  public string? ClientSecret { get; set; }
  public string? ApiKey { get; set; }
}