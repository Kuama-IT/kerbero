namespace Kerbero.Domain.NukiAuthentication.Models.PresentationRequests;

public class NukiRedirectPresentationRequest
{
    public string ClientId { get; }

    public NukiRedirectPresentationRequest(string clientId)
    {
        ClientId = clientId;
    }
}