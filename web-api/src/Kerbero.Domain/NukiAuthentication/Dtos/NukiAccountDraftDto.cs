namespace Kerbero.Domain.NukiAuthentication.Dtos;

public record NukiAccountDraftDto(string ClientId, string RedirectUrl, Guid UserId)
{
}