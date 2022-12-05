using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;

namespace Kerbero.Domain.NukiCredentials.Interfaces;

public interface IGetNukiCredentialsByUserInteractor
{
  /// <summary>
  /// Returns a list of always ready-to-use nuki credential (with fresh token)
  /// </summary>
  /// <param name="userId"></param>
  /// <returns></returns>
  Task<Result<UserNukiCredentialsModel>> Handle(Guid userId);
}