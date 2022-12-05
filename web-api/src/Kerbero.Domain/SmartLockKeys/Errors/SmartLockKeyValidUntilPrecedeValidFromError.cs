using Kerbero.Domain.Common.Errors;

namespace Kerbero.Domain.SmartLockKeys.Errors;

public class SmartLockKeyValidUntilPrecedeValidFromError : KerberoError
{
  public SmartLockKeyValidUntilPrecedeValidFromError(string message = "Valid until must be after valid from") :
    base(message)
  {
  }
}