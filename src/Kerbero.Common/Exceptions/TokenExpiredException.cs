namespace Kerbero.Common.Exceptions;

public class TokenExpiredException: Exception
{
	public TokenExpiredException(): base("The provided Token is expired.") {}
}
