namespace Kerbero.Domain.NukiAuthentication.Models.ExternalRequests;

public class NukiAccountExternalRequest
{
	public string ClientId { get; init; } = null!;
	
	public string? Code { get; init; }

	public string? RefreshToken { get; init; }
}
