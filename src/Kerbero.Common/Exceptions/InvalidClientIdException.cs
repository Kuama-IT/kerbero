namespace Kerbero.Common.Exceptions;

public class InvalidClientIdException: Exception
{
	public InvalidClientIdException(string clientId, Reason reasonType, string? message = default): base(message)
	{
		ClientId = clientId;
		ReasonType = reasonType;
	}

	public override string Message => (string.IsNullOrWhiteSpace(ClientId) ? string.Empty : ClientId + " ") 
	                                  + ReasonType + 
	                                  (string.IsNullOrWhiteSpace(base.Message) ? "" : ", " + base.Message);
	
	private string ClientId { get; }

	private Reason ReasonType { get; }

	public enum Reason
	{
		AlreadyInTheSystem,
		EmptyOrNull,
		InvalidFormat,
	}
}

