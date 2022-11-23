using Kerbero.Domain.SmartLocks.Models;

namespace Kerbero.Domain.SmartLockKeys.Dtos;

public record CreateSmartLockKeyParams(string SmartLockId, DateTime ExpiryDate, int CredentialId, SmartLockProvider SmartLockProvider);
