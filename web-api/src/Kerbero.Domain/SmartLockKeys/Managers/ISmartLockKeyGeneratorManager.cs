using FluentResults;
using Kerbero.Domain.SmartLockKeys.Entities;

namespace Kerbero.Domain.SmartLockKeys.Managers;

public interface ISmartLockKeyGeneratorManager
{
	Result<SmartLockKey> GenerateSmartLockKey(int smartLockId, DateTime expiryDate); // TODO
}
