namespace Kerbero.WebApi.Dtos;

public record OpenSmartLockWithKeyRequestDto(Guid SmartLockKeyId, string KeyPassword);
