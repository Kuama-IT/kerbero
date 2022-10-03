using Kerbero.Domain.Common.Errors;

namespace Kerbero.Domain.NukiAuthentication.Errors.CreateNukiAccountErrors;

public class ExternalServiceUnreachableError: KerberoError
{
	public ExternalServiceUnreachableError(string? message = "External Server returns a timeout or a 5xx message.") : base(message) { }
}
