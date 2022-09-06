using System.ComponentModel.DataAnnotations;

namespace Kerbero.NET.Domain.Entities.Device;

/// <summary>
/// DeviceLog represents a single entry of a log from the Smartlock 
/// </summary>
public class SmartLockLog
{
  public int Id { get; set; }
  
  public DateTime Date { get; set;} 
  
  public string Type { get; set;} = null!;
  
  public string? Description { get; set;} 
  
  public string? OtherParams { get; set;} 
  
  public SmartLock SmartLock { get; set; } = null!;
}