using Kerbero.NET.Domain.Entities.Account;
using System.ComponentModel.DataAnnotations;

namespace Kerbero.NET.Domain.Entities.Providers;
/// <summary>
/// It is a basic class representing an implemented smart-locks provider
/// </summary>
public class SmartLockProvider
{
    public int Id { get; set;}
    
    public string Name { get; set;} = null!;
    
    public string? Logo { get; set;}
    
    public List<SmartLockProviderAccount>? DeviceVendorAccounts { get; set; }
}