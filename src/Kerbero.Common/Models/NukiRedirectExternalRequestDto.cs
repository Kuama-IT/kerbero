namespace Kerbero.Common.Models;

public class NukiRedirectExternalRequestDto
{
	public string ClientId { get; }

	public NukiRedirectExternalRequestDto(string clientId)
	{
		ClientId = clientId;
	}
}
