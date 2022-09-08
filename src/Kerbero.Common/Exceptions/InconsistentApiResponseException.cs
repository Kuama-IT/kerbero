namespace Kerbero.Common.Exceptions;

public class InconsistentApiResponseException: Exception
{
	public InconsistentApiResponseException(dynamic? deserializeObject, string? message = default): base(message)
	{
		DeserializeObj = deserializeObject;
		ReasonType = deserializeObject is null ? Reason.DeserializationReturnNull : Reason.AttributeNotFound;
	}

	public override string Message => (DeserializeObj is null ? string.Empty : DeserializeObj.ToString().Replace('\n', ' ') + ", ") 
	                                  + ReasonType + 
	                                  (string.IsNullOrWhiteSpace(base.Message) ? "" : ", " + base.Message);
	
	private dynamic? DeserializeObj { get; }

	private Reason ReasonType { get; }

	private enum Reason
	{
		DeserializationReturnNull,
		AttributeNotFound
	}
}