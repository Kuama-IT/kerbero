namespace Kerbero.Domain.Common.Errors;

public class DuplicateEntryError: KerberoError
{
	public DuplicateEntryError(string entryType) : base($"The {entryType} already exists, try to update instead.") { }
}
