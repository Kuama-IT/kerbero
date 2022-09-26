namespace Kerbero.Common.Errors.CreateNukiAccountErrors;

public class DuplicateEntryError: KerberoError
{
	public DuplicateEntryError(string entryType) : base($"The {entryType} already exists, try to update instead.") { }
}
