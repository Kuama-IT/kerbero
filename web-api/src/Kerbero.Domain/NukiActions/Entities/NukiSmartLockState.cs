namespace Kerbero.Domain.NukiActions.Entities;

public class NukiSmartLockState
{
    public int Mode { get; set; }
		
    public int State { get; set; }
		
    public int LastAction { get; set; }
		
    public bool BatteryCritical { get; set; }
		
    public bool BatteryCharging { get; set; }
		
    public int BatteryCharge { get; set; }
		
    public int DoorState { get; set; }
		
    public string? OperationId { get; set; }

    public int NukiSmartLockId { get; set; }
    public NukiSmartLock NukiSmartLock { get; set; } = null!;
}