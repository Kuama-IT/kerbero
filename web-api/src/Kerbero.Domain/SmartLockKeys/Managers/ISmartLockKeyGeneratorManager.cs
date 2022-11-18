using FluentResults;
using Kerbero.Domain.SmartLockKeys.Entities;
using Kerbero.Domain.SmartLockKeys.Models.PresentationRequests;

namespace Kerbero.Domain.SmartLockKeys.Managers;

public interface ISmartLockKeyGeneratorManager
{
	Result<SmartLockKey> GenerateSmartLockKey(int smartLockId, DateTime expiryDate); // TODO
}
