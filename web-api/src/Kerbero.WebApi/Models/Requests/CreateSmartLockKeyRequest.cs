namespace Kerbero.WebApi.Models.Requests;

public record CreateSmartLockKeyRequest(string SmartLockId, DateTime ExpiryDate, int CredentialId, string SmartLockProvider)
{
}
