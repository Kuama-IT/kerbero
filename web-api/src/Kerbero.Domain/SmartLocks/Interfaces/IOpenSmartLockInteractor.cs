using FluentResults;
using Kerbero.Domain.Common.Models;

namespace Kerbero.Domain.SmartLocks.Interfaces;

public interface IOpenSmartLockInteractor
{
  Task<Result> Handle(Guid userId, SmartLockProvider smartLockProvider, string smartLockId, int credentialId);
}
