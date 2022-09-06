using Kerbero.NET.Domain.Entities.Providers;
using Kerbero.NET.Domain.Entities.RentInformation;

namespace Kerbero.NET.Domain.Entities.Account;
/// <summary>
/// RentProviderAccount represents a rent service API account
/// </summary>
public class RentProviderAccount
{
  public int Id { get; set; }
  
  public string? Token {get; set;}
  
  public string? RefreshToken {get; set;}
  
  public string ClientId { get; set; } = null!;
  
  public List<Reservation>? Reservations { get; set; }

  public HostAccount HostAccount { get; set; } = null!;

  public RentProvider RentProvider { get; set; } = null!;
}