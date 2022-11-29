namespace Kerbero.WebApi.Models.Requests;

public record OpenSmartLockRequest(int CredentialsId, string SmartLockProvider);