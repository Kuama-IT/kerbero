namespace KerberoWebApi.Models.Device;

// DeviceVendorAccount derive from DeviceVendor class, add the information of an Host account
public class DeviceVendorAccount : DeviceVendor
{
  public string Token { get; }
  public string RefreshToken { get; }
  public string ClientId { get; set; }
  public string? ClientSecret { get; set; }
  public string? ApiKey { get; set; }

  public DeviceVendorAccount(string token, string refreshToken, string clientId, string name) : base(name)
  {
    Token = token;
    RefreshToken = refreshToken;
    ClientId = clientId;
  }
}