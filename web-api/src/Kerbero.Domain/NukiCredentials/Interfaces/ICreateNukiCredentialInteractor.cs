using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;

namespace Kerbero.Domain.NukiCredentials.Interfaces;

public interface ICreateNukiCredentialInteractor
{
  Task<Result<NukiCredentialModel>> Handle(Guid userId, string token);
}