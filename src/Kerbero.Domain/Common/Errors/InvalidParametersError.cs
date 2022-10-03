using Kerbero.Domain.Common.Errors;

namespace Kerbero.Domain.NukiAuthentication.Errors.CommonErrors;

public class InvalidParametersError: KerberoError
{
	public InvalidParametersError(string invalidParameter) : base(
		$"There are missing or wrong parameter in the request: {invalidParameter}.") { }
}
