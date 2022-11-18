using Kerbero.Domain.Common.Errors;

namespace Kerbero.Domain.SmartLockKeys.Errors;

public class UnableToGenerateKeyError: KerberoError
{
	public UnableToGenerateKeyError(string? message = "System is unable to generate a key") : base(message)
	{
	}
}
