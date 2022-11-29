using Kerbero.Domain.Common.Errors;

namespace Kerbero.Domain.SmartLocks.Errors;

public class SmartLockNotFoundError : KerberoError
{
  public SmartLockNotFoundError(string smartLockId) : base(
    $"The id provided ({smartLockId}) is not associated to any SmartLock.")
  {
  }
}