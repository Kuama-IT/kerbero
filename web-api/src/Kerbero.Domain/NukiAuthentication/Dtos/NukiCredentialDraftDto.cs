namespace Kerbero.Domain.NukiAuthentication.Dtos;

public record NukiCredentialDraftDto(string ClientId, string RedirectUrl, Guid UserId)
{
}