using System.ComponentModel.DataAnnotations;
using KerberoWebApi.Models.Device;

namespace KerberoWebApi.Models.Rent;
// A Reservation is downloaded from the Rent provider API and associated with a smartlock
public class Reservation
{
  [Key]
  public int Id { get; set; }
  public DateTime CheckInDate { get; set; }
  public DateTime CheckOutDate { get; set;}
  public string GuestsEmails { get; set;} = null!;
  public string StructureInfos { get; set; } = null!;
  
  // foreign keys
  public int DeviceSmartLockId { get; set;}
  public DeviceSmartLock DeviceSmartLock { get; set; } = null!;
  public int RentProviderAccountId { get; set; }
  public RentProviderAccount RentProviderAccount { get; set; } = null!;
}