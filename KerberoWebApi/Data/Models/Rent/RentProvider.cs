using System.ComponentModel.DataAnnotations;

namespace KerberoWebApi.Models.Rent;
// RentProvider contains the information of a Rent provider service
public class RentProvider
{
  [Key]
  public int Id { get; set;}
  public string Name { get; set;} = null!;
  public string? Logo { get; set;}
    
  // foreign keys
  public List<RentProviderAccount>? DeviceVendorAccounts { get; set; }
}