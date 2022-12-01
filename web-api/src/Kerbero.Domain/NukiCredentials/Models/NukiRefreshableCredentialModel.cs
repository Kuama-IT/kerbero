namespace Kerbero.Domain.NukiCredentials.Models;

public class NukiRefreshableCredentialModel
{
  public string Token { get; set; } = null!;

  public string RefreshToken { get; set; } = null!;

  public int TokenExpiresIn { get; set; }

  public string? Error { get; set; }
  public DateTime CreatedAt { get; set; }
}