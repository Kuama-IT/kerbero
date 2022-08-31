namespace KerberoWebApi.Models
{
    // DeviceLog represents a single entry of a log from the Smartlock
    public class DeviceLog
    {
        public DateTime Date { get; set;}
        public string Type { get; set;}
        public string? Description { get; set;}
        public List<string> OtherParams { get; set;}
        public DeviceLog(DateTime date, string type) { Date = date; Type = type; OtherParams = new List<string>(); }
    }
    public class DeviceLogList
    {
        public List<DeviceLog> Logs {get; set;} = new List<DeviceLog>();
    }
}