using System.Text.Json.Serialization;

namespace Kerbero.Infrastructure.SmartLocks.Models;

public class NukiSmartLockResponse
{
  [JsonPropertyName("smartlockId")]
  public int NukiAccountId { get; set; }
  
  [JsonPropertyName("name")]
  public string Name { get; set; } = null!;
}