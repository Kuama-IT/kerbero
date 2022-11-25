namespace Kerbero.Domain.NukiAuthentication.Dtos;

public class CreateNukiCredentialDraftParams
{
    public string ClientId { get; }
    public Guid UserId { get; }

    public CreateNukiCredentialDraftParams(string clientId, Guid userId)
    {
        ClientId = clientId;
        UserId = userId;
    }
}