using System.Text.Json.Serialization;

namespace Kerbero.Infrastructure.SmartLocks.Models;

public class NukiSmartlockStateResponse
{
  [JsonPropertyName("mode")] public int Mode { get; set; }

  [JsonPropertyName("state")] public int State { get; set; }

  [JsonPropertyName("lastAction")] public int LastAction { get; set; }

  /// <summary>
  /// True if the battery state of the device is critical
  /// </summary>
  [JsonPropertyName("batteryCritical")] public bool BatteryCritical { get; set; }

  /// <summary>
  /// True if a Nuki battery pack in a Smart Lock is currently charging
  /// </summary>
  [JsonPropertyName("batteryCharging")] public bool BatteryCharging { get; set; }

  /// <summary>
  /// Remaining capacity of a Nuki battery pack in %
  /// </summary>
  [JsonPropertyName("batteryCharge")] public int BatteryCharge { get; set; }

  [JsonPropertyName("doorState")] public required ENukiDoorStateResponse DoorStateResponse { get; set; }

  /// <summary>
  /// The operation id - if set it's locked for another operation
  /// </summary>
  [JsonPropertyName("operationId")] public string? OperationId { get; set; }
}