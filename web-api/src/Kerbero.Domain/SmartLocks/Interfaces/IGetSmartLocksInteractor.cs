using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.SmartLocks.Dtos;

namespace Kerbero.Domain.SmartLocks.Interfaces;

public interface IGetSmartLocksInteractor
{
  Task<Result<List<SmartLockDto>>> Handle(List<NukiCredentialModel> nukiCredentials);
}