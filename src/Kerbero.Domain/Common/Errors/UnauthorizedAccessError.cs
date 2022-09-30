namespace Kerbero.Domain.Common.Errors.CreateNukiAccountErrors;

public class UnauthorizedAccessError: KerberoError
{
	public UnauthorizedAccessError(string? message = "The credential are wrong or you can not access to the resource.") : base(message) { }
}
