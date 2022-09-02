using System.ComponentModel.DataAnnotations;

namespace KerberoWebApi.Models.Device;

// DeviceVendorAccount derive from DeviceVendor class, add the information of an Host account
public class DeviceVendorAccount : DeviceVendor
{
  [Key]
  public int Id { get; set; }
  public string? Token { get; set; } = null!; 
  public string? RefreshToken { get; set; } = null!;
  public string ClientId { get; set; } = null!;
  // TODO verify it is necessary, or the same of token 
  public string? ApiKey { get; set; } = null!;
  public string HostId { get; set; } = null!;
}