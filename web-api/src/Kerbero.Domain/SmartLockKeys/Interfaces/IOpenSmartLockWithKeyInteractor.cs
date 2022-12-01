using FluentResults;

namespace Kerbero.Domain.SmartLockKeys.Interfaces;

public interface IOpenSmartLockWithKeyInteractor
{
	Task<Result> Handle(Guid smartLockKeyId, string smartLockKeyPassword);
}
