namespace Kerbero.Common.Errors;

public class InvalidParametersError: KerberoError
{
	public InvalidParametersError(string invalidParameter) : base(
		$"There are missing or wrong parameter in the request: {invalidParameter}.") { }
}
