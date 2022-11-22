namespace Kerbero.Domain.NukiActions.Entities;

// TODO FIX ME
public class NukiSmartLockStateEntity
{
  public int Id { get; set; }
  
  public int Mode { get; set; }

  public int State { get; set; }

  public int LastAction { get; set; }

  public bool BatteryCritical { get; set; }

  public bool BatteryCharging { get; set; }

  public int BatteryCharge { get; set; }

  public int DoorState { get; set; }

  public string? OperationId { get; set; }
}