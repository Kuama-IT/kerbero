using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.SmartLocks.Models;

namespace Kerbero.Domain.SmartLocks.Repositories;

public interface INukiSmartLockRepository
{
  public Task<Result<List<SmartLockModel>>> GetAll(NukiCredentialModel nukiCredentialModel);

  public Task<Result<SmartLockModel>> Get(NukiCredentialModel nukiCredentialModel, string id);

  public Task<Result> Open(NukiCredentialModel nukiCredentialModel, string id);
  public Task<Result> Close(NukiCredentialModel nukiCredentialModel, string id);
}
