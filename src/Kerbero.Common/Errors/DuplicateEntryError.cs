namespace Kerbero.Common.Errors;

public class DuplicateEntryError: KerberoError
{
	public DuplicateEntryError(string? message = default) : base(message) { }
}
