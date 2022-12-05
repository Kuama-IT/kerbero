using FluentResults;
using Kerbero.Domain.SmartLockKeys.Models;

namespace Kerbero.Domain.SmartLockKeys.Interfaces;

public interface IDeleteSmartLockKeyInteractor
{
    public Task<Result<SmartLockKeyModel>> Handle(Guid userId, Guid smartLockId);
}