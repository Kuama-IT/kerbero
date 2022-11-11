using FluentResults;

namespace Kerbero.Domain.Common.Errors;

public class KerberoError: IError
{
	public string? Message { get; }

	public Dictionary<string, object>? Metadata { get; }
	
	public List<IError>? Reasons { get; }
	
	public KerberoError(string? message = "A generic application error occurs.")
	{
		Message = message;
	}
}
