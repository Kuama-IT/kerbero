using KerberoWebApi.Models.Rent;

namespace KerberoWebApi.Models.Device;

// A device is a logic entity which represents the physical smartlock
public class Device
{
  public DeviceVendorAccount VendorInfo { get; set; } = null!;
  public string Model { get; set; } = null!;
  public string Image { get; set; } = null!;
  public string Status { get; set; } = null!;
  public int UnlockNumbers { get; set; } = 0;
  public DateTime LastUnlock { get; set; }
  public List<DeviceKey> ActiveKeys { get; set; } = null!;
  public DeviceLogList Log { get; set; } = null!;
  public List<DeviceKey> ArchivedKeys { get; set; } = null!;
  public List<DeviceKey> DisabledKeys { get; set; } = null!;
  public List<Reservation> UpcomingReservations { get; set; } = null!;
  public Reservation CurrentReservation { get; set; } = null!;
  public List<Reservation> ExpiredReservations { get; set; } = null!;
}