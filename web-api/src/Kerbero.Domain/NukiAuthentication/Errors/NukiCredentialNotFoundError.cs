using Kerbero.Domain.Common.Errors;

namespace Kerbero.Domain.NukiAuthentication.Errors;

public class NukiCredentialNotFoundError: KerberoError
{
	public NukiCredentialNotFoundError(string? message = "NukiCredential not found") : base(message) { }
}
