using Kerbero.Domain.Common.Errors;

namespace Kerbero.Domain.NukiCredentials.Errors;

public class NukiCredentialNotFoundError: KerberoError
{
	public NukiCredentialNotFoundError(string? message = "NukiCredential not found") : base(message) { }
}
