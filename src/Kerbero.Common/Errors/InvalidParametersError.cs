using FluentResults;

namespace Kerbero.Common.Errors;

public class InvalidParametersError: KerberoError
{
	public InvalidParametersError(object parameter,
		string? message = "There are missing or wrong parameter in the request: ") : base(
		message + parameter.ToString())
	{
	}
}
