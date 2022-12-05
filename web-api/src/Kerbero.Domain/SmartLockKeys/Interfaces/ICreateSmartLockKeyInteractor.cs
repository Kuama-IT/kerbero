using FluentResults;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.SmartLockKeys.Models;

namespace Kerbero.Domain.SmartLockKeys.Interfaces;

public interface ICreateSmartLockKeyInteractor
{
  Task<Result<SmartLockKeyModel>> Handle(string smartLockId, DateTime validUntilDate, DateTime validFromDate,
    int credentialId, SmartLockProvider smartLockProvider);
}