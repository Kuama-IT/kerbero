namespace Kerbero.Domain.NukiCredentials.Models;

/// <summary>
/// Represents a nuki credential that will be confirmed later on after a Nuki Authentication process
/// </summary>
public record NukiCredentialDraftModel(Guid UserId, int? Id = null)
{
}