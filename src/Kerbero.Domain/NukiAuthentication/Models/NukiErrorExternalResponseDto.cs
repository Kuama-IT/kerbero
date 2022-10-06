using System.Text.Json.Serialization;

namespace Kerbero.Domain.NukiAuthentication.Models;

public class NukiErrorExternalResponseDto
{
    [JsonPropertyName("error")] 
    public string? Error { get; set; } = "Response is null";

    [JsonPropertyName("error_description")]
    public string? ErrorMessage { get; set; } = "Response from Nuki Api is null, maybe deserialization failed.";
	
    [JsonPropertyName("detailMessage")]
    public string? DetailMessage { get; set; } = "Response from Nuki Api is null, maybe deserialization failed.";
}