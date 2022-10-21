using FluentResults;
using Kerbero.Domain.NukiActions.Entities;

namespace Kerbero.Domain.NukiActions.Repositories;

public interface INukiSmartLockPersistentRepository
{
    Task<Result<NukiSmartLock>> Create(NukiSmartLock nukiSmartLock);
    Task<Result<NukiSmartLock>> GetById(int smartLockId);
}