using System.Text.Json.Serialization;

namespace Kerbero.Data.NukiCredentials.Dtos;

public class NukiOAuthResponseDto
{
  [JsonPropertyName("access_token")]
  public string Token { get; set; } = null!;

  [JsonPropertyName("refresh_token")]
  public string RefreshToken { get; set; } = null!;

  [JsonPropertyName("expires_in")]
  public int TokenExpiresIn { get; set; }

  [JsonPropertyName("token_type")]
  public string TokenType { get; set; } = null!;

  [JsonPropertyName("error")] 
  public string? Error { get; set; } = "Response is null";

  [JsonPropertyName("error_description")]
  public string? ErrorMessage { get; set; } = "Response from Nuki Api is null, maybe deserialization failed.";
}