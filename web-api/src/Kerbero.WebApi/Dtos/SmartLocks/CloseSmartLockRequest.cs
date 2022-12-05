namespace Kerbero.WebApi.Dtos.SmartLocks;

public record CloseSmartLockRequest(int CredentialsId, string SmartLockProvider);