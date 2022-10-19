namespace Kerbero.Domain.NukiAuthentication.Models.ExternalRequests;

public class NukiRedirectExternalRequest
{
	public string ClientId { get; }

	public NukiRedirectExternalRequest(string clientId)
	{
		ClientId = clientId;
	}
}
