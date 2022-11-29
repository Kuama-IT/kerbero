using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.SmartLocks.Models;

namespace Kerbero.Domain.SmartLocks.Repositories;

public interface INukiSmartLockRepository
{
  public Task<Result<List<SmartLock>>> GetAll(NukiCredentialModel nukiCredentialModel);

  public Task<Result<SmartLock>> Get(NukiCredentialModel nukiCredentialModel, string id);
}