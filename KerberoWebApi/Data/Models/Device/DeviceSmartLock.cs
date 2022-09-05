using KerberoWebApi.Models.Rent;
using System.ComponentModel.DataAnnotations;

namespace KerberoWebApi.Models.Device;

// A device is a logic entity which represents the physical smartlock
public class DeviceSmartLock
{
  [Key]
  public int Id { get; set; }
  public int VendorSmartlockId { get; set; }
  
  public string? Model { get; set; } 
  public string? Image { get; set; }
  public string? Status { get; set; }
  public int? UnlockNumbers { get; set; }
  public string? LastAction { get; set; }
  
  public List<DeviceKey>? DeviceKeys { get; set; }
  
  public List<DeviceLog>? DeviceLogs { get; set; }

  public List<Reservation> UpcomingReservations { get; set; } = null!;
  public Reservation CurrentReservation { get; set; } = null!;
  public List<Reservation> ExpiredReservations { get; set; } = null!;

  // foreign keys
  public int DeviceVendorAccountId { get; set; }
  public DeviceVendorAccount DeviceVendorAccount { get; set; } = null!;
}