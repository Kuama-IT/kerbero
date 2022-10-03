using Kerbero.Domain.Common.Errors;

namespace Kerbero.Domain.NukiAuthentication.Errors.CommonErrors;

public class UnknownExternalError: KerberoError
{
	public UnknownExternalError(string? message = "External server returns an unknown error.") : base(message)
	{
	}
}
