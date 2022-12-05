using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;

namespace Kerbero.Domain.NukiCredentials.Interfaces;

public interface IEnsureNukiCredentialBelongsToUserInteractor
{
  Task<Result<NukiCredentialModel>> Handle(Guid userId, int credentialId);
}