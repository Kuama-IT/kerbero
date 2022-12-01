using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;

namespace Kerbero.Domain.NukiCredentials.Interfaces;

public interface ICreateNukiCredentialDraftInteractor
{
  Task<Result<NukiCredentialDraftModel>> Handle(Guid userId);
}