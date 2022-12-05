namespace Kerbero.WebApi.Dtos.NukiCredentials;

public record NukiCredentialResponseDto
{
  public int Id { get; set; }
  public string? Token { get; init; }
}