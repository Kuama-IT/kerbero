namespace Kerbero.Domain.NukiAuthentication.Models;

public class NukiAccountAuthenticatedResponseDto
{
    public int NukiAccountId { get; init; }
    
    public string ClientId { get; init; } = null!;

    public string Token { get; init; } = null!;
}