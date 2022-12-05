using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;

namespace Kerbero.Domain.NukiCredentials.Interfaces;

public interface IGetNukiCredentialInteractor
{
  /// <summary>
  /// Returns an always ready-to-use nuki credential (with fresh token)
  /// </summary>
  /// <param name="nukiCredentialId"></param>
  /// <param name="userId">When provided, it will check if the credential belong to the user</param>
  /// <returns></returns>
  Task<Result<NukiCredentialModel>> Handle(int nukiCredentialId, Guid? userId = null);
}