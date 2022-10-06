namespace Kerbero.Domain.Common.Errors;

public class PersistentResourceNotAvailableError: KerberoError
{
	public PersistentResourceNotAvailableError(string? message = "Persistent resource cannot be reached.") : base(message) { }
}
