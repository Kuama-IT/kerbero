namespace Kerbero.WebApi.Dtos.SmartLockKeys;

public record CreateSmartLockKeyRequestDto(string SmartLockId, DateTime ValidUntilDate, DateTime ValidFromDate, int CredentialId, string SmartLockProvider)
{
}
