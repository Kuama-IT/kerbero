namespace KerberoWebApi.Models.Device;

using System.ComponentModel.DataAnnotations;

public class DeviceVendor
{
    [Key]
    public int Id { get; set;}
    public string Name { get; set;} = null!;
    public string? Logo { get; set;}
}