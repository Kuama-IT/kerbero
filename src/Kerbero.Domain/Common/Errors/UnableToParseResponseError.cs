using Kerbero.Domain.Common.Errors;

namespace Kerbero.Domain.NukiAuthentication.Errors.CreateNukiAccountErrors;

public class UnableToParseResponseError: KerberoError
{
	public UnableToParseResponseError(string? message = "Unknown format for external response.") : base(message) { }
}
