using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;

namespace Kerbero.Domain.NukiCredentials.Interfaces;

public interface IConfirmNukiDraftCredentialsInteractor
{
  Task<Result<NukiCredentialModel>> Handle(string code, Guid draftCredentialUriGuid);
}