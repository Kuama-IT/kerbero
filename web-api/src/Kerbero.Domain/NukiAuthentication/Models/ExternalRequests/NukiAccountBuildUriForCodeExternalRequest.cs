namespace Kerbero.Domain.NukiAuthentication.Models.ExternalRequests;

public class NukiAccountBuildUriForCodeExternalRequest
{
	public string ClientId { get; }

	public NukiAccountBuildUriForCodeExternalRequest(string clientId)
	{
		ClientId = clientId;
	}
}
