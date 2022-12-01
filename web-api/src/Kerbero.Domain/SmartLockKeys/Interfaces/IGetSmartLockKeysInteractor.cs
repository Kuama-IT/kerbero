using FluentResults;
using Kerbero.Domain.SmartLockKeys.Dtos;

namespace Kerbero.Domain.SmartLockKeys.Interfaces;

public interface IGetSmartLockKeysInteractor
{
	public Task<Result<List<SmartLockKeyDto>>> Handle(Guid userId);
}
