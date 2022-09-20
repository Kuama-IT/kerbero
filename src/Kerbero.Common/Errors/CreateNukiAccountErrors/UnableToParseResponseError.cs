namespace Kerbero.Common.Errors;

public class UnableToParseResponseError: KerberoError
{
	public UnableToParseResponseError(string? message = "Unknown format for external response.") : base(message) { }
}
