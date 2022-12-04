using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;

namespace Kerbero.Domain.NukiCredentials.Interfaces;

public interface IGetNukiCredentialsByUserInteractor
{
  Task<Result<List<NukiCredentialModel>>> Handle(Guid userId);
}