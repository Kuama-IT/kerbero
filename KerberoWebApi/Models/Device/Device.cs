namespace KerberoWebApi.Models
{
    // A device is a logic entity which represents the physical smartlock
    public class Device
    {
        public DeviceVendorAccount VendorInfo {get; set;}
        public string? Model {get; set;}
        public string? Image {get; set;}
        public string? Status {get; set;}
        public int UnlockNumbers {get; set;} = 0;
        public DateTime? LastUnlock {get; set;}
        public List<DeviceKey>? ActiveKeys {get; set;}
        public DeviceLogList? Log {get; set;}
        public List<DeviceKey>? ArchivedKeys {get; set;}
        public List<DeviceKey>? DisabledKeys {get; set;}
        public List<Reservation>? UpcomingReservations { get; set;}
        public Reservation? CurrentReservation { get; set;}
        public List<Reservation>? ExpiredReservations { get; set;}
        public Device(DeviceVendorAccount vendorAccount) { VendorInfo = vendorAccount; }
    }
}