namespace Kerbero.WebApi.Models.Requests;

public record CloseSmartLockWithKeyRequest(Guid SmartLockKeyId, string KeyPassword);
