namespace Kerbero.Domain.NukiAuthentication.Dtos;

public class CreateNukiAccountDraftParams
{
    public string ClientId { get; }
    public Guid UserId { get; }

    public CreateNukiAccountDraftParams(string clientId, Guid userId)
    {
        ClientId = clientId;
        UserId = userId;
    }
}