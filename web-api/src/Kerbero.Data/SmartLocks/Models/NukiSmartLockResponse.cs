﻿using System.Text.Json.Serialization;

namespace Kerbero.Data.SmartLocks.Models;

public class NukiSmartLockResponse
{
  [JsonPropertyName("smartlockId")] public int NukiAccountId { get; set; }

  [JsonPropertyName("name")] public required string Name { get; set; }

  [JsonPropertyName("state")] public required NukiSmartlockStateResponse State { get; set; }

  [JsonPropertyName("type")] public required ENukiSmartLockType Type { get; set; }
}