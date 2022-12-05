namespace Kerbero.WebApi.Dtos;

public record CreateSmartLockKeyRequestDto(string SmartLockId, DateTime ValidUntilDate, DateTime ValidFromDate, int CredentialId, string SmartLockProvider)
{
}
