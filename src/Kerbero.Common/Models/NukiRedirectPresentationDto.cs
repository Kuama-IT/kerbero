namespace Kerbero.Common.Models;

public class NukiRedirectPresentationDto
{
	public Uri RedirectUri { get; }

	public NukiRedirectPresentationDto(Uri redirectUri)
	{
		RedirectUri = redirectUri;
	}
}
