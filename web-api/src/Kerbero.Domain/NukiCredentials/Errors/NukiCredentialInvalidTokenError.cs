using Kerbero.Domain.Common.Errors;

namespace Kerbero.Domain.NukiCredentials.Errors;

public class NukiCredentialInvalidTokenError : KerberoError
{
  public NukiCredentialInvalidTokenError(string? message = "You provided an invalid Nuki Api Token") : base(message)
  {
  }
}