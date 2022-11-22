namespace Kerbero.Domain.NukiAuthentication.Models;

/// <summary>
/// Represents a nuki account that will be confirmed later on after a Nuki Authentication process
/// </summary>
public record NukiCredentialDraft(string ClientId, string RedirectUrl, Guid UserId);