namespace KerberoWebApi.Models.Device;

public abstract class DeviceVendor
{
    public string Name;
    public string? Logo { get; set;}
    protected DeviceVendor(string name) { Name = name; }
}