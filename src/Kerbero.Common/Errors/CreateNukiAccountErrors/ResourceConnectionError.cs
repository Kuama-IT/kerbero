namespace Kerbero.Common.Errors;

public class ResourceConnectionError: KerberoError
{
	public ResourceConnectionError(string? message = default) : base(message) { }
}
