using Kerbero.Domain.Common.Errors;

namespace Kerbero.Domain.NukiActions.Errors;

public class SmartLockNotFoundError: KerberoError
{
    public SmartLockNotFoundError(): base("The id provided is not associated to any SmartLock.")
    {
    }
}