using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.SmartLocks.Models;

namespace Kerbero.Domain.SmartLocks.Interfaces;

public interface IGetSmartLocksInteractor
{
  Task<Result<List<SmartLockWithCredentialModel>>> Handle(List<NukiCredentialModel> nukiCredentials);
}