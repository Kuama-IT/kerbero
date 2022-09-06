using Kerbero.NET.Domain.Entities.Account;
using System.ComponentModel.DataAnnotations;

namespace Kerbero.NET.Domain.Entities.Providers;

/// <summary>
/// RentProvider contains the information of a Rent provider service
/// </summary>
public class RentProvider
{
    public int Id { get; set;}
  
    public string Name { get; set;} = null!;
  
    public string? Logo { get; set;}
    
    public List<RentProviderAccount>? DeviceVendorAccounts { get; set; }
}