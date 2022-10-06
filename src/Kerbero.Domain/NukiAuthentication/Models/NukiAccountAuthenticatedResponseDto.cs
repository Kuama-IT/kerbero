namespace Kerbero.Domain.NukiAuthentication.Models;

public class NukiAccountAuthenticatedResponseDto
{
    public int NukiAccountId { get; init; }
    
    public string ClientId { get; init; }
    
    public string Token { get; init; }
}