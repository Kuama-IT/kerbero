using Kerbero.Domain.Common.Errors;

namespace Kerbero.Domain.NukiActions.Errors;

public class SmartLockNotReachableError: KerberoError
{
    public SmartLockNotReachableError(): base("The Nuki SmartLock is not reachable")
    {
    }
}