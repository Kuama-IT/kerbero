using FluentResults;
using Kerbero.Domain.SmartLockKeys.Entities;

namespace Kerbero.Domain.SmartLockKeys.Repositories;

public interface ISmartLockKeyPersistentRepository
{
	Task<Result<SmartLockKey>> Create(SmartLockKey smartLockKey);
}
