namespace KerberoWebApi.Models
{
    public class Reservation
    {
        public DateTime CheckInDate { get; set;}
        public DateTime CheckOutDate { get; set;}
        public List<string> GuestsEmails { get; set;}
        public List<string>? StructureInfos { get; set;}
        public Device AssociatedDevice { get; } // da eliminare il riferimento se non necessario
        public Reservation(DateTime checkin, DateTime checkout, string[] emails, ref Device device) { CheckInDate = checkin; CheckOutDate = checkout; GuestsEmails = emails.ToList<string>(); AssociatedDevice = device; }
    }
}