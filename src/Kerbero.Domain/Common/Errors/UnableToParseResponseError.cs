namespace Kerbero.Domain.Common.Errors.CreateNukiAccountErrors;

public class UnableToParseResponseError: KerberoError
{
	public UnableToParseResponseError(string? message = "Unknown format for external response.") : base(message) { }
}
