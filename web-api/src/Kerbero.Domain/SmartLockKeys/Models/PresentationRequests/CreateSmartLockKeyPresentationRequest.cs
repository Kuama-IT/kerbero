namespace Kerbero.Domain.SmartLockKeys.Models.PresentationRequests;

public record CreateSmartLockKeyPresentationRequest(int SmartLockId, DateTime ExpiryDate);
