using Kerbero.Domain.Common.Errors;

namespace Kerbero.Domain.NukiAuthentication.Errors;

public class NukiAccountNotFoundError: KerberoError
{
	public NukiAccountNotFoundError(string? message = "No NukiAccountFound") : base(message) { }
}
