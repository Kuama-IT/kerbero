using FluentResults;
using Kerbero.Domain.NukiActions.Entities;

namespace Kerbero.Domain.NukiActions.Repositories;

public interface INukiSmartLockPersistentRepository
{
    Task<Result<NukiSmartLockEntity>> Create(NukiSmartLockEntity nukiSmartLockEntity);
    Task<Result<NukiSmartLockEntity>> GetById(int smartLockId);
}