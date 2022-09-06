using Kerbero.NET.Domain.Entities.Providers;

namespace Kerbero.NET.Domain.Entities.Account;

/// <summary>
/// A basic identity entity which represents an Host
/// </summary>
public class HostAccount
{
    /// <summary>
    /// Host Id primary key
    /// </summary>
    public int Id { get; set; }

    public string Name { get; set; }  = null!;
    
    public string Surname { get; set; } = null!;
    
    public string? Icon { get; set; }
    
    public string Email { get; set; } = null!;
    
    public string Password { get; set; } = null!;
    
    public List<RentProviderAccount>? RentProviderAccounts { get; set; }
    
    public List<SmartLockProviderAccount>? SmartLockProviderAccounts { get; set; }

}
