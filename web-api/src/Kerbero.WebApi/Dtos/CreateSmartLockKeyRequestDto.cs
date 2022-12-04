namespace Kerbero.WebApi.Dtos;

public record CreateSmartLockKeyRequestDto(string SmartLockId, DateTime ExpiryDate, int CredentialId, string SmartLockProvider)
{
}
