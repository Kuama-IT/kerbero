using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;

namespace Kerbero.Domain.NukiCredentials.Interfaces;

public interface IDeleteNukiCredentialInteractor
{
  public Task<Result<NukiCredentialModel>> Handle(Guid userId, int nukiCredentialId);
}