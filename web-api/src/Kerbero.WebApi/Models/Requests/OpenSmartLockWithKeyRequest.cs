namespace Kerbero.WebApi.Models.Requests;

public record OpenSmartLockWithKeyRequest(Guid SmartLockKeyId, string KeyPassword);
