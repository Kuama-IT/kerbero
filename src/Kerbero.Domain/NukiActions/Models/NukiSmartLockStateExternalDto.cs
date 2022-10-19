using System.Text.Json.Serialization;

namespace Kerbero.Domain.NukiActions.Models;

public class NukiSmartLockStateExternalDto
{
	[JsonPropertyName("mode")]
	public int Mode { get; set; }
	
	[JsonPropertyName("state")]
	public int State { get; set; }
	
	[JsonPropertyName("lastAction")]
	public int LastAction { get; set; }
	
	[JsonPropertyName("batteryCritical")]
	public bool BatteryCritical { get; set; }
	
	[JsonPropertyName("batteryCharging")]
	public bool BatteryCharging { get; set; }
	
	[JsonPropertyName("batteryCharge")]
	public int BatteryCharge { get; set; }
	
	[JsonPropertyName("doorState")]
	public int DoorState { get; set; }
	
	[JsonPropertyName("operationId")]
	public string? OperationId { get; set; }
}