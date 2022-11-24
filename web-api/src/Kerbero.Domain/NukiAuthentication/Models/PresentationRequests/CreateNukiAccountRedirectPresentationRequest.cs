namespace Kerbero.Domain.NukiAuthentication.Models.PresentationRequests;

public class CreateNukiAccountRedirectPresentationRequest
{
    public string ClientId { get; }

    public CreateNukiAccountRedirectPresentationRequest(string clientId)
    {
        ClientId = clientId;
    }
}