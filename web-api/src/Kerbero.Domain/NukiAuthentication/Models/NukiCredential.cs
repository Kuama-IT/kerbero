namespace Kerbero.Domain.NukiAuthentication.Models;

public class NukiCredential
{
  public int? Id { get; set; }
  public string Token { get; set; } = null!;
  public string RefreshToken { get; set; } = null!;
  public int TokenExpiringTimeInSeconds { get; set; }

  public DateTime ExpireDate => CreatedAt.AddSeconds(TokenExpiringTimeInSeconds);
  
  public DateTime CreatedAt { get; set; }
  public string TokenType { get; set; } = null!;
  public string ClientId { get; set; } = null!;
}