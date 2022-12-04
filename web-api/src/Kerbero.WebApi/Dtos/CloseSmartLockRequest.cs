namespace Kerbero.WebApi.Dtos;

public record CloseSmartLockRequest(int CredentialsId, string SmartLockProvider);