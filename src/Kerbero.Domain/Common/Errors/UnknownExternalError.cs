namespace Kerbero.Domain.Common.Errors.CommonErrors;

public class UnknownExternalError: KerberoError
{
	public UnknownExternalError(string? message = "External server returns an unknown error.") : base(message)
	{
	}
}
