using Kerbero.Domain.Common.Errors;

namespace Kerbero.Domain.NukiAuthentication.Errors.CreateNukiAccountErrors;

public class PersistentResourceNotAvailableError: KerberoError
{
	public PersistentResourceNotAvailableError(string? message = "Persistent resource cannot be reached.") : base(message) { }
}
