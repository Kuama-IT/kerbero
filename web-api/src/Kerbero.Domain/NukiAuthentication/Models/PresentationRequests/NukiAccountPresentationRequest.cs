namespace Kerbero.Domain.NukiAuthentication.Models.PresentationRequests;

public class NukiAccountPresentationRequest
{
    public string ClientId { get; init; } = null!;
	
    public string? Code { get; init; }

    public string? RefreshToken { get; init; }
}