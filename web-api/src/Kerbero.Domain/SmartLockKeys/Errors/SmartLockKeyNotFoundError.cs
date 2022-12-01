using Kerbero.Domain.Common.Errors;

namespace Kerbero.Domain.SmartLockKeys.Errors;

public class SmartLockKeyNotFoundError: KerberoError
{
	public SmartLockKeyNotFoundError(string? message = "SmartLock key not found.") : base(message)
	{
	}
}
