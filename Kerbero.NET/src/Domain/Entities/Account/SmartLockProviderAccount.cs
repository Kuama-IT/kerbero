using System.ComponentModel.DataAnnotations;
using Kerbero.NET.Domain.Entities.Device;
using Kerbero.NET.Domain.Entities.Providers;

namespace Kerbero.NET.Domain.Entities.Account;
/// <summary>
/// SmartLockProviderAccount represents a basic account for a smart-lock API provider
/// </summary>
public class SmartLockProviderAccount
{
  public int Id { get; set; }
  
  public string? Token { get; set; }
  
  public string? RefreshToken { get; set; }
  
  public string ClientId { get; set; } = null!;
  
  public List<SmartLock>? DeviceSmartLocks { get; set; }

  public HostAccount HostAccount { get; set; } = null!;
  
  public SmartLockProvider SmartLockProvider { get; set; } = null!;
}