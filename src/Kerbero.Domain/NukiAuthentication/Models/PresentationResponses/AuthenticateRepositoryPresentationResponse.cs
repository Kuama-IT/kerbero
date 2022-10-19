namespace Kerbero.Domain.NukiAuthentication.Models.PresentationResponses;

public class AuthenticateRepositoryPresentationResponse
{
    public int NukiAccountId { get; init; }
    
    public string ClientId { get; init; } = null!;

    public string Token { get; init; } = null!;
}