namespace Kerbero.WebApi.Dtos.SmartLocks;

public record OpenSmartLockRequestDto(int CredentialsId, string SmartLockProvider);