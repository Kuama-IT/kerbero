namespace Kerbero.Domain.NukiAuthentication.Models;

public class NukiOAuthRequest
{
  public string ClientId { get; set; } = null!;

  public string? Code { get; set; }

  public string? RefreshToken { get; set; }
}