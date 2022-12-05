namespace Kerbero.WebApi.Dtos.SmartLockKeys;

public record CloseSmartLockWithKeyRequestDto(Guid SmartLockKeyId, string KeyPassword);
