using FluentResults;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.SmartLockKeys.Dtos;
using Kerbero.Domain.SmartLocks.Models;

namespace Kerbero.Domain.SmartLockKeys.Interfaces;

public interface ICreateSmartLockKeyInteractor
{
  Task<Result<SmartLockKeyDto>> Handle(string smartLockId, DateTime expiryDate, int credentialId,
    SmartLockProvider smartLockProvider);
}
