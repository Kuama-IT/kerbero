namespace KerberoWebApi.Models
{
    // DeviceVendor represents the basic information of a Smartlock vendor
    public abstract class DeviceVendor
    {
        public string Name;
        public string? Logo { get; set;}
        protected DeviceVendor(string name) { Name = name; }
    }
}