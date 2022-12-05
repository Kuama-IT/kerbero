namespace Kerbero.WebApi.Dtos.SmartLockKeys;

public record OpenSmartLockWithKeyRequestDto(Guid SmartLockKeyId, string KeyPassword);
