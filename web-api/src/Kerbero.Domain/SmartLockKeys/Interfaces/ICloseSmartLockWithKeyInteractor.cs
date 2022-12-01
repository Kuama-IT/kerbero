using FluentResults;

namespace Kerbero.Domain.SmartLockKeys.Interfaces;

public interface ICloseSmartLockWithKeyInteractor
{
	Task<Result> Handle(Guid smartLockKeyId, string smartLockKeyPassword);
}
