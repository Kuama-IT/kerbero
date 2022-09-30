namespace Kerbero.Domain.NukiAuthentication.Models;

public class NukiRedirectExternalRequestDto
{
	public string ClientId { get; }

	public NukiRedirectExternalRequestDto(string clientId)
	{
		ClientId = clientId;
	}
}
