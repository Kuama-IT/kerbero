namespace Kerbero.Domain.NukiAuthentication.Models.PresentationResponses;

public class NukiRedirectPresentationResponse
{
	public Uri RedirectUri { get; }

	public NukiRedirectPresentationResponse(Uri redirectUri)
	{
		RedirectUri = redirectUri;
	}
}
