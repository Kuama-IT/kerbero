using System.ComponentModel.DataAnnotations;

namespace KerberoWebApi.Models.Device;
// DeviceLog represents a single entry of a log from the Smartlock
public class DeviceLog
{
  [Key]
  public int Id { get; set; }
  public DateTime Date { get; set;} 
  public string Type { get; set;} = null!;
  public string? Description { get; set;} 
  public string? OtherParams { get; set;} 
  
  // foreign key
  public int DeviceSmartLockId { get; set; } 
  public DeviceSmartLock DeviceSmartLock { get; set; } = null!;
  
}