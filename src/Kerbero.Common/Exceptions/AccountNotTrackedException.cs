namespace Kerbero.Common.Exceptions;

public class AccountNotTrackedException: Exception
{
	public AccountNotTrackedException(): base("The account does not provide an identification") { }
}
