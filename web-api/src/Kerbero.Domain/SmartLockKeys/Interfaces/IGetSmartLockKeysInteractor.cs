using FluentResults;
using Kerbero.Domain.SmartLockKeys.Models;

namespace Kerbero.Domain.SmartLockKeys.Interfaces;

public interface IGetSmartLockKeysInteractor
{
	public Task<Result<List<SmartLockKeyModel>>> Handle(Guid userId);
}
