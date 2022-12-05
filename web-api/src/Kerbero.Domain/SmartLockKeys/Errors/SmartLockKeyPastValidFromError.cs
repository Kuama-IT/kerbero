using Kerbero.Domain.Common.Errors;

namespace Kerbero.Domain.SmartLockKeys.Errors;

public class SmartLockKeyPastValidFromError: KerberoError
{
	public SmartLockKeyPastValidFromError(string message = "You cannot create keys that can be used before today"): base(message)
	{
	}
}
