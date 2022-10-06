namespace Kerbero.Domain.NukiAuthentication.Models;

public class NukiAccountExternalRequestDto
{
	public string ClientId { get; init; } = null!;
	
	public string? Code { get; init; }

	public string? RefreshToken { get; init; }
}
