namespace Kerbero.WebApi.Dtos;

public record NukiCredentialResponseDto
{
  public int Id { get; set; }
  public string? Token { get; init; }
}