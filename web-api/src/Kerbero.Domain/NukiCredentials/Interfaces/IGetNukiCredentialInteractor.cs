using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;

namespace Kerbero.Domain.NukiCredentials.Interfaces;

public interface IGetNukiCredentialInteractor
{
  Task<Result<NukiCredentialModel>> Handle(int nukiCredentialId);
}