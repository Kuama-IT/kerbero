namespace Kerbero.Domain.NukiActions.Models.PresentationRequest;

public class NukiSmartLocksPresentationRequest
{
	public NukiSmartLocksPresentationRequest(string token)
	{
		Token = token;
	}

	public string Token { get; }
}
