using FluentResults;
using Kerbero.Domain.SmartLockKeys.Models;
using Kerbero.Domain.SmartLocks.Models;

namespace Kerbero.Domain.SmartLockKeys.Interfaces;

public interface IUpdateSmartLockKeyValidityInteractor
{
  Task<Result<SmartLockKeyModel>> Handle(Guid userId, Guid smartLockKeyGuid, DateTime validUntil, DateTime validFrom);
}