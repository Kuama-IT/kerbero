namespace Kerbero.Domain.NukiAuthentication.Dtos;

public record NukiCredentialDto
{
	public int Id { get; set; }
	public string ClientId { get; set; } = null!;

	public string Token { get; init; } = null!;
}
