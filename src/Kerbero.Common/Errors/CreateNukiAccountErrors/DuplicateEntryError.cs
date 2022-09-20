namespace Kerbero.Common.Errors.CreateNukiAccountErrors;

public class DuplicateEntryError: KerberoError
{
	public DuplicateEntryError(string? message = default) : base(message) { }
}
