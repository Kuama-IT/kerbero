namespace Kerbero.Common.Exceptions;

public class InvalidTokenException: Exception
{
	public InvalidTokenException(): base("The provider account contains an invalid token.") { }
}
