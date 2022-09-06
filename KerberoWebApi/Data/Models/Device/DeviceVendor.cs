using System.ComponentModel.DataAnnotations.Schema;

namespace KerberoWebApi.Models.Device;

using System.ComponentModel.DataAnnotations;

public class DeviceVendor
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set;}
    public string Name { get; set;} = null!;
    public string? Logo { get; set;}
    
    // foreign keys
    public List<DeviceVendorAccount>? DeviceVendorAccounts { get; set; }
}