using System.ComponentModel.DataAnnotations;

namespace KerberoWebApi.Models.Rent;
// Derived from RentProvider, it represents an account for the rent provider service
public class RentProviderAccount
{
  [Key]
  public int Id { get; set; }
  public string? Token {get; set;}
  public string? RefreshToken {get; set;}
  public string ClientId { get; set; } = null!;
  public List<Reservation>? Reservations { get; set; }
  
  // foreign keys
  public int HostId { get; set; }
  public Host Host { get; set; } = null!;
}