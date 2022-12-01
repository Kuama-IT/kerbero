using Kerbero.Domain.Common.Errors;

namespace Kerbero.Domain.SmartLockKeys.Errors;

public class SmartLockKeyExpiredError: KerberoError
{
	public SmartLockKeyExpiredError(string message = "The provided smartlock key is expired."): base(message)
	{
	}
}
