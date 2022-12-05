using System.Text.Json.Serialization;

namespace Kerbero.Infrastructure.NukiCredentials.Dtos;

public class NukiAccountResponseDto
{
  [JsonPropertyName("accountId")] public int AccountId { get; set; }

  [JsonPropertyName("email")] public string Email { get; set; } = null!;

  [JsonPropertyName("name")] public string Name { get; set; } = null!;
}