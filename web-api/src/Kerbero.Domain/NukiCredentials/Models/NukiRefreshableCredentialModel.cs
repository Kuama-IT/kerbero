namespace Kerbero.Domain.NukiCredentials.Models;

/// <summary>
/// Credentials returned by Nuki rest api at the end of a Nuki oauth flow
/// </summary>
/// <param name="Token"></param>
/// <param name="RefreshToken"></param>
/// <param name="TokenExpiresIn"></param>
/// <param name="Error"></param>
/// <param name="CreatedAt"></param>
public record NukiRefreshableCredentialModel
(
  string Token,
  string RefreshToken,
  int TokenExpiresIn,
  string? Error,
  DateTime CreatedAt
)
{
}